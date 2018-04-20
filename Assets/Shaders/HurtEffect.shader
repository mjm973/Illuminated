Shader "Effect/Hurt" {
  Properties {
    _Color ("Color", Color) = (0, 0, 0, 1)
    _Blend ("Blend Factor", Float) = 0
    _MainTex ("Frame Texture", 2D) = "white" {}
  }

  SubShader {
    Pass {
      Tags {

      }

      Cull Off ZWrite Off ZTest Always
      Blend One Zero

      CGPROGRAM

      #pragma target 3.0

      #pragma vertex Vert
      #pragma fragment Frag

      #include "HurtEffect.cginc"

      ENDCG
    }
  }
}
