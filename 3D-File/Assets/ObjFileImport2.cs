using UnityEngine;
using System.Collections;

public class ObjFileImport2 : MonoBehaviour {

	public Texture2D myTexture;

	string objPath = @"C:\E.obj"; // AirPlane

	Mesh m;

	void Start () {
		InitMesh ();
		ObjImporter objImporter = new ObjImporter();
		m = objImporter.ImportFile (objPath);

		m.RecalculateBounds();
		m.RecalculateNormals();

//		m= GetComponent<ObjImporter>().ImportFile(objPath);
	}

	// 初始化
	void InitMesh(){
		gameObject.AddComponent ("MeshRenderer");
		gameObject.AddComponent ("MeshFilter");
		
		Material mt = renderer.material;
		mt.mainTexture = myTexture;
		mt.shader = Shader.Find("Mobile/Unlit (Supports Lightmap)");
		
		MeshRenderer mr = GetComponent <MeshRenderer>();
		mr.material = mt;
		
		MeshFilter mf = GetComponent <MeshFilter>();
		m = new Mesh ();
		mf.mesh = m;
	}

}
