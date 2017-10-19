//Hard Surface Shader Package, Written for the Unity engine by Bruno Rime: http://www.behance.net/brunorime brunorime@gmail.com
#ifndef HARD_SURFACE_PRO_SCREEN_SPACE_REFELCTION_INCLUDED
#define HARD_SURFACE_PRO_SCREEN_SPACE_REFELCTION_INCLUDED

struct v2f
                {
					float4 pos : SV_POSITION;
					float3 viewDir : TEXCOORD0;
					float2 uv_BumpMap : TEXCOORD1;
					float3 TtoV0  : TEXCOORD2;
					float3 TtoV1  : TEXCOORD3;
					float4 GPscreenPos : TEXCOORD4;
					float3 vertworldpos : TEXCOORD5;
                }; 
		
		// define variables
			sampler2D _BumpMap;
			sampler2D _GrabTexture;
			sampler2D _Spec_Gloss_Reflec_Masks;
			float4 _BumpMap_ST;
			fixed  _FrezPow;
			fixed  _FrezFalloff;
			fixed  _Metalics;
			fixed4 _Color;
			fixed _Reflection;
			fixed _RefScale;
			
				
		v2f vert (appdata_tan v)
                {
                    v2f o;
					
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.vertworldpos = mul(_Object2World, v.vertex).xyz;
					//o.viewDir.xzy = ObjSpaceViewDir(v.vertex);
					
					o.uv_BumpMap = TRANSFORM_TEX( v.texcoord, _BumpMap );
					
					o.GPscreenPos.xy = (float2(o.pos.x, o.pos.y) + o.pos.w) * 0.5;
					o.GPscreenPos.zw = o.pos.zw;
					 
                    TANGENT_SPACE_ROTATION; 
					o.viewDir.xyz = mul( rotation, ObjSpaceViewDir( v.vertex ) );
					o.TtoV0 = mul(rotation, UNITY_MATRIX_IT_MV[0].xyz);
					o.TtoV1 = mul(rotation, UNITY_MATRIX_IT_MV[1].xyz);

                    return o;
                }
				
		half4 frag( v2f IN ) : COLOR 
		{
		  #ifdef HardsurfaceNormal
				fixed3 Bumpnormal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		   #else
				fixed3 Bumpnormal = fixed3(0,0,1);
		   #endif

			#ifdef HardsurfaceSpecular
				#ifdef ShaderModel3
					fixed ReflecMask = tex2D(_Spec_Gloss_Reflec_Masks, IN.uv_BumpMap).g;
				#endif
			#endif			

			//screenspaced normal direction
			half2 vn;
			vn.x = dot(IN.TtoV0, Bumpnormal);
			vn.y = dot(IN.TtoV1, Bumpnormal);
			
			// lays over the normal in the direction its biased to.
			half2 absvn = abs(vn);
			half maxvn = max(absvn.x,absvn.y);
			maxvn = 1 / maxvn;
			vn = vn * maxvn;
			
			
			#ifdef SHADER_API_D3D9
				vn.y *= _ProjectionParams.x;
			#endif
			
			// calculates VeiwAngle falloff, distance to vert, and base blend for realtime relection
			float3 viewVect = _WorldSpaceCameraPos - IN.vertworldpos;
			half dist = length (viewVect);
			half refscale =min (1/dist,dist/1);
			//half refscale =1/dist;
			
			IN.viewDir = normalize(IN.viewDir);
			half SurfAngle= dot(IN.viewDir,Bumpnormal);
			SurfAngle = pow(SurfAngle,2 * (1-refscale));
	
			
			// hard coded falloff for RefCapture to mask some screenspace artifacts
			fixed blend = 1 - saturate(SurfAngle * 1.1 );
		
			// screen space coords for capture
			fixed2 grabTexcoord = IN.GPscreenPos.xy / IN.GPscreenPos.w; 
			
			// warps the screensapce coords by the screenspace normals, wich are tweeked by distance and viewangle
			fixed2 screenuv;
			screenuv = grabTexcoord + (vn * SurfAngle * (1-SurfAngle * refscale ));

			fixed2 screenedges =  abs(screenuv * 2 - 1);
			screenedges = 1 - (screenedges * screenedges) ;
			
			// fresnal RefCapture falloff
			fixed frez = pow(1-SurfAngle,_FrezFalloff)*_FrezPow * 0.0009765625 ;
			
			#ifdef SHADER_API_D3D9
				screenuv.y = 1 - screenuv.y;
			#endif	
			

			half3 RefCapture = tex2D(_GrabTexture,screenuv).rgb;
			
			// uses the lowest falloff value available
			#ifdef HardsurfaceSpecular
				#ifdef ShaderModel3
					fixed RefCaptureblend = min((_Reflection + frez),blend) * ReflecMask;
				#else
					fixed RefCaptureblend = min((_Reflection + frez),blend);
				#endif
			#else
				fixed RefCaptureblend = min((_Reflection + frez),blend);
			#endif		
			
			RefCapture.rgb *= RefCaptureblend;
			fixed ReflectiveLum = Luminance(RefCapture);
			
			ReflectiveLum *= ReflectiveLum;
			
			// based on metalics value use a tinted or untinted RefCapture
			fixed3 reflectionColor = lerp ( RefCapture , RefCapture*lerp(_Color.rgb ,1,ReflectiveLum*ReflectiveLum),_Metalics);
			fixed reflectionOpacity = RefCaptureblend * min(screenedges.x,screenedges.y);
			
			return float4 (reflectionColor,reflectionOpacity);
		}
	  
#endif
