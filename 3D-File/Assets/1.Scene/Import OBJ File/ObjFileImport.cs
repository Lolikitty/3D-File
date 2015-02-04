using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ObjFileImport : MonoBehaviour {

	public Texture2D myTexture;

	string objPath = @"C:\F.obj"; // AirPlane
	string uvTexturePath = @"C:\F.png";

	Mesh m;

	List<Vector3> v = new List<Vector3>();
	List<Vector2> vt = new List<Vector2>();
	List<Vector3> vn = new List<Vector3>();
	List<int [,]> f = new List<int [,]>();
	List<int> t = new List<int>();

	int _3sidesCount = 0;
	int _4sidesCount = 0;
	int all3sidesCount = 0;

	Texture2D uvTexture;

	IEnumerator Start(){

		WWW w = new WWW ("file://"+uvTexturePath);
		yield return w;
		uvTexture = w.texture;
		w.Dispose ();

		InitMesh ();
		ReadFile ();
		CreateMesh ();
	}

	// 初始化
	void InitMesh(){
		gameObject.AddComponent ("MeshRenderer");
		gameObject.AddComponent ("MeshFilter");
		
		Material mt = renderer.material;
		mt.mainTexture = uvTexture;
		mt.shader = Shader.Find("Mobile/Unlit (Supports Lightmap)");
//		mt.shader = Shader.Find("Diffuse");
		
		MeshRenderer mr = GetComponent <MeshRenderer>();
		mr.material = mt;
		
		MeshFilter mf = GetComponent <MeshFilter>();
		m = new Mesh ();
		mf.mesh = m;
	}

	void ReadFile(){

		if(!File.Exists(objPath)){
			Debug.LogError("此路徑找不到 obj 檔案：" + objPath);
			return;
		}

		string line;
		int iv = 0;
		using(StreamReader file = new StreamReader(objPath)){
			while((line = file.ReadLine()) != null){
				string head = line.Split(' ')[0];
				switch(head){
				case "#" :
				case "o" :
				case "s" :
				case "" :
					continue;
				}

				string [] stringArray = line.Split(' ');

				// 匯入頂點
				if(head == "v"){
					if(stringArray.Length == 4){
						float x = float.Parse(stringArray[1]);
						float y = float.Parse(stringArray[2]);
						float z = float.Parse(stringArray[3]);
						v.Add(new Vector3 (x, y, z));
					}else{
						print ("error 1");
						break;
					}
				}else if(head == "vn"){
					if(stringArray.Length == 4){
						float x = float.Parse(stringArray[1]);
						float y = float.Parse(stringArray[2]);
						float z = float.Parse(stringArray[3]);
						vn.Add(new Vector3 (x, y, z));
					}else{
						print ("error 2");
						break;
					}
				}else if(head == "f"){
					// 如果輸入是四邊形
					if(stringArray.Length == 5){
						int [,] value = new int[4,3];

						string [] sa1 = stringArray[1].Split('/');
						string [] sa2 = stringArray[2].Split('/');
						string [] sa3 = stringArray[3].Split('/');
						string [] sa4 = stringArray[4].Split('/');

						value[0,0] = sa1[0] == "" ? -1 : int.Parse(sa1[0]);
						value[0,1] = sa1[1] == "" ? -1 : int.Parse(sa1[1]);
						value[0,2] = sa1[2] == "" ? -1 : int.Parse(sa1[2]);

						value[1,0] = sa2[0] == "" ? -1 : int.Parse(sa2[0]);
						value[1,1] = sa2[1] == "" ? -1 : int.Parse(sa2[1]);
						value[1,2] = sa2[2] == "" ? -1 : int.Parse(sa2[2]);

						value[2,0] = sa3[0] == "" ? -1 : int.Parse(sa3[0]);
						value[2,1] = sa3[1] == "" ? -1 : int.Parse(sa3[1]);
						value[2,2] = sa3[2] == "" ? -1 : int.Parse(sa3[2]);

						value[3,0] = sa4[0] == "" ? -1 : int.Parse(sa4[0]);
						value[3,1] = sa4[1] == "" ? -1 : int.Parse(sa4[1]);
						value[3,2] = sa4[2] == "" ? -1 : int.Parse(sa4[2]);

						_4sidesCount++;

						f.Add(value);
					}
					// 如果輸入是三角形
					else if(stringArray.Length == 4){
						int [,] value = new int[3,3];
						
						string [] sa1 = stringArray[1].Split('/');
						string [] sa2 = stringArray[2].Split('/');
						string [] sa3 = stringArray[3].Split('/');
						
						value[0,0] = sa1[0] == "" ? -1 : int.Parse(sa1[0]);
						value[0,1] = sa1[1] == "" ? -1 : int.Parse(sa1[1]);
						value[0,2] = sa1[2] == "" ? -1 : int.Parse(sa1[2]);
						
						value[1,0] = sa2[0] == "" ? -1 : int.Parse(sa2[0]);
						value[1,1] = sa2[1] == "" ? -1 : int.Parse(sa2[1]);
						value[1,2] = sa2[2] == "" ? -1 : int.Parse(sa2[2]);
						
						value[2,0] = sa3[0] == "" ? -1 : int.Parse(sa3[0]);
						value[2,1] = sa3[1] == "" ? -1 : int.Parse(sa3[1]);
						value[2,2] = sa3[2] == "" ? -1 : int.Parse(sa3[2]);

						_3sidesCount++;

						f.Add(value);
					}else{
						print ("error 3");
						break;
					}
				}else if(head == "vt"){
					float x = float.Parse(stringArray[1]);
					float y = float.Parse(stringArray[2]);
					vt.Add(new Vector2(x, y));
				}
			}
		}
		all3sidesCount = _3sidesCount + _4sidesCount * 2;
	}
	
	void CreateMesh () {

		print ("點 數量：" + v.Count);
		print ("顯示 面 數量：" + f.Count);
		print ("顯示三角形 面 數量：" + _3sidesCount);
		print ("顯示四邊形 面 數量：" + _4sidesCount);
		print ("實際三角形 面 數量：" + all3sidesCount);

		foreach(int [,] ff in f){
			int vCount = ff.GetLength(0);
			// 建立四邊形
			if(vCount == 4){
				t.Add(ff[0, 0]-1);
				t.Add(ff[1, 0]-1);
				t.Add(ff[3, 0]-1);
				
				t.Add(ff[2, 0]-1);
				t.Add(ff[3, 0]-1);
				t.Add(ff[1, 0]-1);
			}
			// 建立三角形
			else if(vCount == 3){
				t.Add(ff[0, 0]-1);
				t.Add(ff[1, 0]-1);
				t.Add(ff[2, 0]-1);
			}
		}

		// 建立法線
		Vector3 [] n = new Vector3[v.Count];

////		int i = 0;
//		int nValue;
//
//		foreach(int [,] ff in f){
//			int vCount = ff.GetLength(0);
//
////			if(i>=n.Length){
////				break;
////			}
//
//			// 建立四邊形法線
//			if(vCount == 4){
////				print (i + " : " + vn[ff[3, 2]-1]);
////				n [i] = vn[ff[3, 2]-1] * -1;
//			}
//			// 建立三角形法線
//			else if(vCount == 3){
////				n [i] = vn[ff[2, 2]-1] * -1;
////				print (i + " : " + vn[ff[2, 2]-1]);
//
//			}
////			n [i] = new Vector3(0,0,1);
////			i++;
//		}
//
//		for(int i = 0; i < v.Count; i++){
//			n [i] = new Vector3(0,0,1);
//		}

//		for(int i = 0; i < all3sidesCount; i++){
//			foreach(int [,] ff in f){
////				print ( i + " : " + ff[i,0] + "   " + ff[i,1] + "   " + ff[i,2]);
////				int nValue = ff[i,2];
////				if(nValue != -1){
////					n [i] = vn[nValue-1] * -1;
////				}
//			}
//		}

//		Vector2 [] uvs = new Vector2[8];

		Vector2 [] uvs = new Vector2[v.Count];

		for(int i = 0; i< uvs.Length; i++){
//			uvs[i] = vt[i];
		}

//		int i = 0;
//		foreach(int [,] ff in f){
//			int vCount = ff.GetLength(0);
//			// 建立四邊形
//			if(vCount == 4){
//				float uvX1 = vt[ff[0, 1]-1].x;
//				float uvY1 = vt[ff[0, 1]-1].y;
//
//				float uvX2;
//				float uvY2;
//
//				float uvX3;
//				float uvY3;
//
//				float uvX4;
//				float uvY4;
//
//				uvs[i] = new Vector2( );
//			}
//			// 建立三角形
//			else if(vCount == 3){
//				t.Add(ff[0, 0]-1);
//				t.Add(ff[1, 0]-1);
//				t.Add(ff[2, 0]-1);
//			}
//			i++;
//		}
//
//		print (v.Count);

//		for(int i = 0; i < uv.Length; i++){
//			uv [i] = new Vector2(0, 10);
//		}

//		uv [0] = new Vector2(1, 0);
//		uv [1] = new Vector2(0, 1); //
//		uv [2] = new Vector2(1, 0);
//		uv [3] = new Vector2(0, 1);

//		uv [4] = new Vector2(1, 0); //
//		uv [5] = new Vector2(0, 1); //
//		uv [6] = new Vector2(1, 0); //
//		uv [7] = new Vector2(0, 1);

		// 套用
		m.vertices = v.ToArray();
		m.triangles = t.ToArray();
		m.normals = n;
		m.uv = uvs;

		m.RecalculateBounds();
		m.RecalculateNormals();
	}

}
