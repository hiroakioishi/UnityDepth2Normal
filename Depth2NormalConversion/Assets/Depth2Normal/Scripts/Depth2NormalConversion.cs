using UnityEngine;
using System.Collections;

public class Depth2NormalConversion : MonoBehaviour {

	private RenderTexture _colorBuffer;
	private RenderTexture _depthBuffer;
	private RenderTexture _normalMap;

	public Material Height2NormalMapConvertMat;
	public float    BumpHeightScale = -1.0f;
	public float    TexelSizeScale  =  1.0f;

	public bool     IsDebug = true;

	public RenderTexture ColorBuffer {
		get {
			return this._colorBuffer;
		}
	}

	public RenderTexture DepthBuffer {
		get {
			return this._depthBuffer;
		}
	}

	public RenderTexture NormalMap {
		get {
			return this._normalMap;
		}
	}

	void Start () {
		_init ();
	}
	
	void Update () {

		Height2NormalMapConvertMat.SetFloat ("_BumpHeightScale", BumpHeightScale);
		Height2NormalMapConvertMat.SetFloat ("_TexelSizeScale",  TexelSizeScale );
		Graphics.Blit (_depthBuffer, _normalMap, Height2NormalMapConvertMat);

	}

	void _init () {
			
		Camera cam = GetComponent <Camera> ();
		cam.depthTextureMode = DepthTextureMode.Depth;

		// Color Buffer
		_colorBuffer = new RenderTexture (Screen.width, Screen.height,  0, RenderTextureFormat.ARGBHalf);
		_colorBuffer.Create ();

		// Depth Buffer
		_depthBuffer = new RenderTexture (Screen.width, Screen.height, 24, RenderTextureFormat.Depth);
		_depthBuffer.Create ();

		// Normal Map
		_normalMap   = new RenderTexture (Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf); 
		_normalMap.Create ();

		// Set Color & Depth Buffer
		cam.SetTargetBuffers (_colorBuffer.colorBuffer, _depthBuffer.depthBuffer);
	}

	void OnPostRender () {
		// No Render Target : Output on Screen
		Graphics.SetRenderTarget (null);
	}

	void OnDestroy () {
		_colorBuffer.Release ();
		_colorBuffer = null;
		_depthBuffer.Release ();
		_depthBuffer = null;
		_normalMap.Release ();
		_normalMap   = null;
	}

	void OnGUI () {
		if (!IsDebug) return;
		int   size = 128;
		float asp  = _colorBuffer.width * 1.0f / _colorBuffer.height * 1.0f;
		GUI.DrawTexture (new Rect (             0, 0, size * asp, size), _colorBuffer);
		GUI.DrawTexture (new Rect (size * 1 * asp, 0, size * asp, size), _depthBuffer);
		GUI.DrawTexture (new Rect (size * 2 * asp, 0, size * asp, size), _normalMap  );
	}
}
