
Shader "World" {
   Properties {
      _ColorAtlas ("ColorAtlas", 2D) = "white" {}
      _ScreenTexture ("ScreenTexture", 2D) = "white" {}
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
         uniform sampler2D _ColorAtlas;	
         uniform sampler2D _ScreenTexture;	
         uniform float _Shininess;
         uniform float _Offset;
         uniform float _MyTime;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posWorld : TEXCOORD0;
            float4 tex : TEXCOORD1; 
            float3 normalWorld : TEXCOORD3;
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
			
            output.posWorld = mul(modelMatrix, input.vertex);
			output.posWorld.y=GetCurve(output.posWorld);

			float3 tang =GetTangent(output.posWorld);
			float3 binorm=GetBinormal(output.posWorld);
			float3 norm = cross(tang,binorm);

            output.normalWorld = normalize(
               mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);

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
			float3 clr = tex2D(_ColorAtlas,input.tex.xy);
			float screenDot = pow(max(dot(input.normalScreen,float3(0,0,-1)),0),0.5);
			float4 text = tex2D(_ScreenTexture,input.posScreen.xy+input.normalScreen.xy+floor(_Time.y*1)*0.1);
			float4 modText = (1-((1-text*0.4)*(1-screenDot)));
			//return (1-((1-text*0.5)*(1-screenDot)));
			return float4(clr*modText,1);
         }
         ENDCG
      }
	}
}