//Hard Surface Shader Package, Written for the Unity engine by Bruno Rime: http://www.behance.net/brunorime brunorime@gmail.com
#ifndef HARD_SURFACE_INCLUDED
#define HARD_SURFACE_INCLUDED

samplerCUBE _Cube;

#ifdef HardsurfaceNormal
	sampler2D _BumpMap;
#endif

#ifdef HardsurfaceDiffuse
	sampler2D _MainTex;	
#endif
	
#ifdef HardsurfaceSpecular
	sampler2D _Spec_Gloss_Reflec_Masks;
#endif

fixed4 _Color;

#ifdef ShaderModel3
	fixed _Shininess;
	fixed _Gloss;
#endif

fixed _Reflection;
fixed _FrezPow;
fixed _FrezFalloff;
fixed _EdgeAlpha;


fixed _Metalics;

struct Input {
	float3 worldNormal;
	float2 uv_MainTex;
	# ifdef HardsurfaceNormal
		float2 uv_BumpMap;
	#endif
	float3 worldRefl;
	INTERNAL_DATA
};

void surf (Input IN, inout SurfaceOutput o) {
	
	#ifdef HardsurfaceSpecular
		fixed3 SpecGlossRefMask = tex2D(_Spec_Gloss_Reflec_Masks, IN.uv_BumpMap).rgb;
	#endif
	
	#ifdef HardsurfaceSpecular
		#ifdef ShaderModel3
			_Shininess *= pow(SpecGlossRefMask.r,3);
			_Shininess = max (_Shininess , .005);
			_Gloss *= SpecGlossRefMask.g;
		#endif
		_Reflection *= SpecGlossRefMask.b;
	#endif
	
	#ifdef HardsurfaceNormal
		#ifdef HardsurfaceBackface
			#ifdef SHADER_API_D3D9 
				#ifdef ShaderModel3
					o.Normal = UnpackNormal(tex2Dlod(_BumpMap, float4(IN.uv_MainTex,1,(1-_Shininess)*3)));
				#else
					o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				#endif
			#else
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			#endif
		#else
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		#endif
	#else
		   o.Normal = fixed3(0,0,1);
	#endif
	
	#ifdef HardsurfaceDiffuse
		_Color.rgba *= tex2D(_MainTex, IN.uv_MainTex).rgba;
	#endif
	
	//Claculate Reflection vector
	half3 worldRefl = normalize(WorldReflectionVector (IN, o.Normal));
	half3 worldNormal = WorldNormalVector  (IN, o.Normal);
	
	fixed SurfAngle= abs(dot (worldRefl,worldNormal));
	fixed frez = pow(1-SurfAngle,_FrezFalloff) ;
	
	_Color.a *= (1-(frez * _EdgeAlpha ));
	
	frez*=_FrezPow * 0.0009765625;
	
	#ifdef HardsurfaceBackface
		fixed Diffusion = (1-_Color.a);
		Diffusion *= Diffusion;
		_Reflection *= Diffusion;
		frez *= Diffusion;
		#ifdef ShaderModel3
			_Shininess *=  Diffusion ;
			_Gloss *= Diffusion;
			_SpecColor.rgb = lerp(_Color.rgb, _SpecColor.rgb, Diffusion); 
		#endif	
	#endif
	

	// Decalre variables for platform specific variations;
	fixed4 CubeTex;
	
	#ifdef SHADER_API_D3D9
		#ifdef ShaderModel3
			CubeTex = texCUBElod(_Cube,float4(worldRefl,(1-_Shininess)*6));
		#else
			CubeTex = texCUBE(_Cube,worldRefl);
		#endif
	#else
		CubeTex = texCUBE(_Cube,worldRefl);
	#endif
	

	
	// Add Fresnel Falloff to Reflection & calculate Reflection Luminace for Blending with diffuse
	#ifdef HardsurfaceSpecular
		fixed Reflection = saturate(_Reflection + (frez * SpecGlossRefMask.b));
	#else 
		fixed Reflection = saturate(_Reflection + frez);
	#endif
	CubeTex.rgb *= Reflection;
	fixed ReflectiveLum = Luminance(CubeTex.rgb);
	
	ReflectiveLum *= ReflectiveLum;
	
	#ifdef ShaderModel3
		o.Specular = _Shininess ;
		#ifdef HardsurfaceSpecular
			o.Gloss = saturate(_Gloss + (frez * SpecGlossRefMask.g));
		#else
			o.Gloss = saturate(_Gloss + frez);
		#endif
	#endif
		
	o.Emission = lerp (CubeTex.rgb ,CubeTex.rgb*lerp(_Color.rgb ,1,ReflectiveLum),_Metalics);
	o.Alpha =  saturate(_Color.a + ReflectiveLum);
	o.Albedo =  _Color.rgb *  (1-Reflection); 
	

}

#endif
