//Hard Surface Shader Package, Written for the Unity engine by Bruno Rime: http://www.behance.net/brunorime brunorime@gmail.com
#ifndef HARD_SURFACE_PRO_SCREEN_SPACE_REFRACTION_INCLUDED
#define HARD_SURFACE_PRO_SCREEN_SPACE_REFRACTION_INCLUDED

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
			float4 _BumpMap_ST;
			fixed  _FrezPow;
			fixed  _FrezFalloff;
			fixed  _Metalics;
			fixed4 _Color;
			fixed _Reflection;
			fixed _RefScale;
			half _Density;
		
		v2f vert (appdata_tan v)
                {
                    v2f o;
					
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.vertworldpos = mul(_Object2World, v.vertex).xyz;
					
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

			//screenspaced normal direction
			half2 vn;
			vn.x = dot(IN.TtoV0, Bumpnormal);
			vn.y = dot(IN.TtoV1, Bumpnormal);
			
			#ifdef SHADER_API_D3D9
				vn.y *= -_ProjectionParams.x;
			#endif
			
			// calculates VeiwAngle falloff, distance to vert, and base blend for realtime relection
			float3 viewVect = _WorldSpaceCameraPos - IN.vertworldpos;
			half dist = length (viewVect);
			half refscale = min (1/dist,dist/1);
			
			IN.viewDir = normalize(IN.viewDir);
			half SurfAngle= dot(IN.viewDir,Bumpnormal);
			
			SurfAngle *= (1 - _Density);
			_Density *= 20;
			
			// screen space coords for capture
			fixed2 grabTexcoord = IN.GPscreenPos.xy / IN.GPscreenPos.w; 
			
			#ifdef SHADER_API_D3D9
				grabTexcoord.y = 1-grabTexcoord.y;
			#endif
			
			vn =  vn * (1-SurfAngle) * -_Density * refscale;
		
			
			half2 screenedges =  abs((saturate(grabTexcoord + vn)) * 2 - 1);
			screenedges = 1 - (screenedges * screenedges ) ;
			
			vn = vn * screenedges;
			
			fixed3 refAberationColor;
			
			#ifdef ShaderModel3
				half2 vnr = vn * .96;
				half2 vng = vn * .98;
				
				refAberationColor.r = tex2D(_GrabTexture,grabTexcoord+vnr).r;
				refAberationColor.g = tex2D(_GrabTexture,grabTexcoord+vng).g;
				refAberationColor.b = tex2D(_GrabTexture,grabTexcoord+vn).b;
			#else
				refAberationColor = tex2D(_GrabTexture,grabTexcoord+vn).rgb;
			#endif
			
			return float4 (refAberationColor,1);

		}
	  
#endif
