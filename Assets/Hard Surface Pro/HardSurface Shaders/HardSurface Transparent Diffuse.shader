//Hard Surface Shader Package, Written for the Unity engine by Bruno Rime: http://www.behance.net/brunorime brunorime@gmail.com
Shader "HardSurface/Hardsurface/Transparent Diffuse"{

Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
	_Shininess ("Shininess", Range (0.01, 3)) = 1.5
	_Gloss("Gloss", Range (0.00, 1)) = .5
	_Reflection("Reflection", Range (0.00, 1)) = 0.5
	_Cube ("Reflection Cubemap", Cube) = "Black" { TexGen CubeReflect }
	_FrezPow("Fresnel Reflection",Range(0,1024)) = 512
	_FrezFalloff("Fresnal/EdgeAlpha Falloff",Range(0,10)) = 4
	_EdgeAlpha("Edge Alpha",Range(0,1)) = 0
	_Metalics("Metalics",Range(0,1)) = .5
	
	
	_MainTex ("Diffuse(RGB) Alpha(A)",2D) = "White" {}
	
}

	SubShader {
		
		// Zprime backfaces
		Pass {
			Tags {"Queue"="Geometry" "IgnoreProjector"="True" "LightMode" = "Always"}
			zwrite on Cull front
			colormask 0
		}
		
		// Backfaces
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		UsePass "Hidden/Hardsurface Pro Back Transparent Diffuse/FORWARD"
		
		// Frontface Zprime
		Pass {
			Tags {"Queue"="Transparent" "IgnoreProjector"="True" "LightMode" = "Always"}
			zwrite on Cull back
			colormask 0
		}

		 // Front Faces
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		UsePass "Hidden/Hardsurface Pro Front Transparent Diffuse/FORWARD"
		
	} 
}
