
Shader "World" {
   Properties {
      _BumpMap ("Normal Map", 2D) = "bump" {}
      _ColorAtlas ("ColorAtlas", 2D) = "white" {}
      _ScreenTexture ("ScreenTexture", 2D) = "white" {}
      _Color ("Diffuse Material Color", Color) = (1,1,1,1) 
      _SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
      _Shininess ("Shininess", Float) = 10
      _Offset ("Offset", Float) = 0
      _MyTime ("Time", Float) = 0
   }
   SubShader {
      Pass {      
         Tags { "LightMode" = "ForwardBase" } 
            // pass for ambient light and first light source
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag  
 
         #include "UnityCG.cginc"
         uniform float4 _LightColor0; 
            // color of light source (from "Lighting.cginc")
 
         // User-specified properties
         uniform sampler2D _BumpMap;	
         uniform sampler2D _ColorAtlas;	
         uniform sampler2D _ScreenTexture;	
         uniform float4 _BumpMap_ST;
         uniform float4 _Color; 
         uniform float4 _SpecColor; 
         uniform float _Shininess;
         uniform float _Offset;
         uniform float _MyTime;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
            float3 normal : NORMAL;
            float4 tangent : TANGENT;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posWorld : TEXCOORD0;
               // position of the vertex (and fragment) in world space 
            float4 tex : TEXCOORD1;
            float3 tangentWorld : TEXCOORD2;  
            float3 normalWorld : TEXCOORD3;
            float3 binormalWorld : TEXCOORD4;
			float3 normalScreen : TEXCOORD5;
			float3 posScreen : TEXCOORD6;
         };
         float GetCurve(float3 p)
		 {
			//return p.y;
			float o = p.y-(pow((p.z+_Offset)*0.1,2));
			o+=+sin(p.z*0.1+p.x*0.1+_MyTime)*1;
			o+=sin(p.z*0.3+_MyTime)*0.2 ;
			return o;
		 }
		 float3 GetTangent(float3 p){
			float3 a,b;
			a.z=p.z+0.001;
			a.x=p.x;
			a.y=0;
			a.y=GetCurve(a);

			b.z=p.z-0.001;
			b.x=p.x;
			b.y=0;
			b.y=GetCurve(b);
			return normalize(a-b);
		 }
		 float3 GetBinormal(float3 p){
			float3 a,b;
			a.x=p.x+0.001;
			a.z=p.z;
			a.y=0;
			a.y=GetCurve(a);

			b.x=p.x-0.001;
			b.z=p.z;
			b.y=0;
			b.y=GetCurve(b);
			return normalize(a-b);
		 }

         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = _Object2World;
            float4x4 modelMatrixInverse = _World2Object; 
               // unity_Scale.w is unnecessary 
			////////////
			
            output.posWorld = mul(modelMatrix, input.vertex);
			output.posWorld.y=GetCurve(output.posWorld);

			float3 tang =GetTangent(output.posWorld);
			float3 binorm=GetBinormal(output.posWorld);
			float3 norm = cross(tang,binorm);

            output.tangentWorld = normalize(
               mul(modelMatrix, float4(input.tangent.xyz, 0.0)).xyz);
            output.normalWorld = normalize(
               mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
            output.binormalWorld = normalize(
               cross(output.normalWorld, output.tangentWorld) 
               * input.tangent.w); // tangent.w is specific to Unity

			float3 finalNormal = output.normalWorld;
			output.normalWorld=normalize(
				finalNormal.y*norm+
				finalNormal.z*tang+
				finalNormal.x*binorm);
			output.normalScreen = mul(UNITY_MATRIX_VP, output.normalWorld);

			
			float4 clipSpace =  mul(UNITY_MATRIX_MVP, input.vertex);
			clipSpace/=clipSpace.w;
			output.posScreen = float3(clipSpace.x,clipSpace.y,0);

            output.tex = input.texcoord;
            output.pos = mul(UNITY_MATRIX_VP, output.posWorld);
            return output;
         }



         float4 frag(vertexOutput input) : COLOR
         {
            // in principle we have to normalize tangentWorld,
            // binormalWorld, and normalWorld again; however, the 
            // potential problems are small since we use this 
            // matrix only to compute "normalDirection", 
            // which we normalize anyways
 
            float4 encodedNormal = tex2D(_BumpMap, 
               _BumpMap_ST.xy * input.tex.xy + _BumpMap_ST.zw);
            float3 localCoords = float3(2.0 * encodedNormal.a - 1.0, 
                2.0 * encodedNormal.g - 1.0, 0.0);
            localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));
               // approximation without sqrt:  localCoords.z = 
               // 1.0 - 0.5 * dot(localCoords, localCoords);
 
            float3x3 local2WorldTranspose = float3x3(
               input.tangentWorld, 
               input.binormalWorld, 
               input.normalWorld);
            float3 normalDirection = 
               normalize(mul(localCoords, local2WorldTranspose));
 
            float3 viewDirection = normalize(
               _WorldSpaceCameraPos - input.posWorld.xyz);
            float3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            } 
            else // point or spot light
            {
               float3 vertexToLightSource = 
                  _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
			float3 clr = tex2D(_ColorAtlas,input.tex.xy);
            float3 ambientLighting = 
               UNITY_LIGHTMODEL_AMBIENT.rgb * clr;
 
            float3 diffuseReflection = 
               attenuation * _LightColor0.rgb * clr
               * max(0.0, dot(normalDirection, lightDirection));
 
            float3 specularReflection;
            if (dot(normalDirection, lightDirection) < 0.0) 
               // light source on the wrong side?
            {
               specularReflection = float3(0.0, 0.0, 0.0); 
                  // no specular reflection
            }
            else // light source on the right side
            {
               specularReflection = attenuation * _LightColor0.rgb 
                  * _SpecColor.rgb * pow(max(0.0, dot(
                  reflect(-lightDirection, normalDirection), 
                  viewDirection)), _Shininess);
            }
			float screenDot = pow(max(dot(input.normalScreen,float3(0,0,-1)),0),0.5);
			//return float4(screenDot,screenDot,screenDot,1);
			float4 text = tex2D(_ScreenTexture,input.posScreen.xy+input.normalScreen.xy+floor(_Time.y*1)*0.1);
			float4 modText = (1-((1-text*0.4)*(1-screenDot)));
			//return (1-((1-text*0.5)*(1-screenDot)));
			return float4(clr*modText,1);
            return float4((ambientLighting + diffuseReflection 
               + specularReflection)*modText, 1.0);
         }
         ENDCG
      }
	}
}