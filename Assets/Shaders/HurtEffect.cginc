#if !defined(HURT_EFFECT_INC)
#define HURT_EFFECT_INC

float4 _Color;
float _Blend;
sampler2D _MainTex;

struct VData {
  float4 vertex : POSITION;
  float4 uv : TEXCOORD0;
};

struct Interpolators {
  float4 pos : SV_POSITION;
  float4 uv : TEXCOORD0;
  float3 wPos : TEXCOORD1;
  //float4 screenPos : TEXCOORD2;
};

Interpolators Vert (VData v) {
  Interpolators i;

  i.pos = UnityObjectToClipPos(v.vertex);
  i.uv = v.uv;
  i.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;

  return i;
}

float4 Frag (Interpolators i) : SV_TARGET {
  float3 albedo = tex2D(_MainTex, i.uv.xy).rgb;
  float a = clamp(_Blend, 0, 1);
  albedo = albedo * (1-a) + _Color * a;

  return float4(albedo, 1);
}

#endif
