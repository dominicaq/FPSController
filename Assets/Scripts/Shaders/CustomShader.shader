Shader "Unlit/CustomShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", COLOR) = (1,1,1,1)
        _Brightness("Brightness", Range(0,1)) = 0.3
        _AmbientColor("Ambient Color", COLOR) = (1,1,1,1)
        
        [Header(Specular)] 
        [Toggle(Enable Specular)]
        _EnableSpecular ("Enable Specular", Float) = 0
        _SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
        _Glossiness("Glossiness", Float) = 32

        [Header(Rim)] 
        [Toggle(Enable rim lighting)]
        _EnableRim ("Enable rim lighting", Float) = 0
        _RimThreshold("Rim", Float) = 0.1
    }
    SubShader
    {
        Tags {"LightMode" = "ForwardBase" 
              "RenderType" = "Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma multi_compile_fwdbase 
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;

                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                SHADOW_COORDS(2)
                float4 pos : SV_POSITION;
                float3 normalDir : NORMAL;

                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Brightness;
            float4 _AmbientColor;
            float4 _Color;

            float _EnableSpecular;
            float4 _SpecularColor;
            float _Glossiness;

            float _EnableRim;
            float _RimThreshold;

            float Diffuse(float3 normal, float3 lightDir)
            {
                float NdotL = dot(normal, lightDir);
                float halfLambert = NdotL * 0.5 + 0.5;
                return max(0.0 ,halfLambert);
            }

            float4 Rim(float3 normal, float3 view)
            {
                return 1 - dot(view, normal);
            }


            //https://digitalerr0r.net/2015/10/26/unity-5-shader-programming-3-specular-light/
            float4 Phong(float3 reflection, float3 view)
            {
				float specAngle = max(0, dot(reflection, view));
				float specularIntensity = pow(specAngle, _Glossiness * _Glossiness);
                return specularIntensity * _SpecularColor;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normalDir = UnityObjectToWorldNormal(v.normal);

                TRANSFER_SHADOW(o)
                return o;
            }

            // https://github.com/shantanubhadoria/Unity3D-Shaders-Basic/blob/master/Shaders/5b%20Point%20Lights.shader
            fixed4 frag (v2f i) : SV_Target
            {
                float shadow = SHADOW_ATTENUATION(i);
                float3 viewDirection = normalize(-_WorldSpaceCameraPos.xyz);
                float3 normal = normalize(i.normalDir);

                float3 lightDirection;
				float attenuation = 1.0;

                if( _WorldSpaceLightPos0.w == 0.0 ) 
                {
					attenuation = 1.0;
					lightDirection = normalize( _WorldSpaceLightPos0.xyz );
				} 
                else 
                {
					float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.pos.xyz; 
					float distance = length( fragmentToLightSource );
					attenuation = 1.0 / distance;
					lightDirection = normalize( fragmentToLightSource );
				}

                // Diffuse
                float kd = attenuation * Diffuse(normal, lightDirection) * shadow;
                kd *= _LightColor0 * _AmbientColor;

                // Specular
                float3 reflectDir = reflect(-lightDirection, normal);
                float4 ks = attenuation * Phong(reflectDir, viewDirection);

                float4 rimDot = attenuation * Rim(normal, viewDirection) * _RimThreshold;

                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                fixed4 final = kd + _Brightness;

                if(_EnableSpecular)
                    final = final + ks;

                if(_EnableRim)
                    final = final + rimDot;

                return col * final;
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
