using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PP_HurtEffect : MonoBehaviour {

    Material mat;
    float alpha = 0f;

	static PP_HurtEffect hurtEffect = null;
	public static PP_HurtEffect HurtEffect {
		get { return hurtEffect; }
	}

    [Range(0f, 1f)]
    public float fadeTime = 0.5f;
    float lastTime = 0f;

	public Color hurtColor = Color.red;

    // Use this for initialization
    void Start() {
        mat = new Material(Shader.Find("Effect/Hurt"));
		hurtEffect = this;
    }

    // Update is called once per frame
    void Update() {
        if (alpha > 0.0f) {
            alpha = Mathf.Lerp(1f, 0f, (Time.time - lastTime) / fadeTime);
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        mat.SetFloat("_Blend", alpha);
		mat.SetColor ("_Color", hurtColor);
        Graphics.Blit(source, destination, mat);
    }

    public void Trigger() {
        lastTime = Time.time;
        alpha = 1f;
    }
}
