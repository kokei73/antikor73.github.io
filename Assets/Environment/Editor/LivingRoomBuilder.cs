#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class LivingRoomBuilder : EditorWindow
{
    [MenuItem("Character Tools/Generate Living Room Environment")]
    public static void BuildLivingRoom()
    {
        GameObject roomRoot = new GameObject("LivingRoom_Environment");

        // 1. Materials
        Material floorMat = new Material(Shader.Find("Standard"));
        floorMat.color = new Color(0.9f, 0.85f, 0.75f); // Light wood/laminate

        Material wallMat = new Material(Shader.Find("Standard"));
        wallMat.color = new Color(0.95f, 0.95f, 0.95f); // Off-white

        Material sofaMat = new Material(Shader.Find("Standard"));
        sofaMat.color = new Color(0.3f, 0.4f, 0.5f); // Soft blue/grey

        Material rugMat = new Material(Shader.Find("Standard"));
        rugMat.color = new Color(0.8f, 0.8f, 0.8f);

        Material tvMat = new Material(Shader.Find("Standard"));
        tvMat.color = Color.black;
        tvMat.SetFloat("_Glossiness", 0.9f);

        Material woodMat = new Material(Shader.Find("Standard"));
        woodMat.color = new Color(0.4f, 0.2f, 0.1f);

        Material plantMat = new Material(Shader.Find("Standard"));
        plantMat.color = new Color(0.2f, 0.6f, 0.2f);

        Material windowMat = new Material(Shader.Find("Standard"));
        windowMat.color = new Color(0.8f, 0.9f, 1.0f, 0.3f);
        windowMat.SetFloat("_Mode", 3); // Transparent

        // 2. Room Structure
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(roomRoot.transform);
        floor.transform.localScale = new Vector3(10, 0.2f, 10);
        floor.transform.position = new Vector3(0, -0.1f, 0);
        floor.GetComponent<MeshRenderer>().material = floorMat;

        GameObject backWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backWall.name = "BackWall";
        backWall.transform.SetParent(roomRoot.transform);
        backWall.transform.localScale = new Vector3(10, 4, 0.5f);
        backWall.transform.position = new Vector3(0, 2, 5);
        backWall.GetComponent<MeshRenderer>().material = wallMat;

        GameObject sideWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sideWall.name = "SideWall";
        sideWall.transform.SetParent(roomRoot.transform);
        sideWall.transform.localScale = new Vector3(0.5f, 4, 10);
        sideWall.transform.position = new Vector3(-5, 2, 0);
        sideWall.GetComponent<MeshRenderer>().material = wallMat;

        GameObject windowWall1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        windowWall1.name = "WindowWall_Bottom";
        windowWall1.transform.SetParent(roomRoot.transform);
        windowWall1.transform.localScale = new Vector3(0.5f, 1f, 10);
        windowWall1.transform.position = new Vector3(5, 0.5f, 0);
        windowWall1.GetComponent<MeshRenderer>().material = wallMat;

        GameObject windowGlass = GameObject.CreatePrimitive(PrimitiveType.Cube);
        windowGlass.name = "WindowGlass";
        windowGlass.transform.SetParent(roomRoot.transform);
        windowGlass.transform.localScale = new Vector3(0.1f, 2f, 4f);
        windowGlass.transform.position = new Vector3(5, 2f, 0);
        windowGlass.GetComponent<MeshRenderer>().material = windowMat;

        // 3. Furniture
        GameObject rug = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rug.name = "Rug";
        rug.transform.SetParent(roomRoot.transform);
        rug.transform.localScale = new Vector3(4, 0.05f, 3);
        rug.transform.position = new Vector3(0, 0.02f, 1);
        rug.GetComponent<MeshRenderer>().material = rugMat;

        GameObject sofaSeat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sofaSeat.name = "Sofa_Seat";
        sofaSeat.transform.SetParent(roomRoot.transform);
        sofaSeat.transform.localScale = new Vector3(3, 0.5f, 1);
        sofaSeat.transform.position = new Vector3(0, 0.4f, 3);
        sofaSeat.GetComponent<MeshRenderer>().material = sofaMat;

        GameObject sofaBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sofaBack.name = "Sofa_Back";
        sofaBack.transform.SetParent(roomRoot.transform);
        sofaBack.transform.localScale = new Vector3(3, 1f, 0.3f);
        sofaBack.transform.position = new Vector3(0, 0.9f, 3.35f);
        sofaBack.GetComponent<MeshRenderer>().material = sofaMat;

        GameObject tvStand = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tvStand.name = "TV_Stand";
        tvStand.transform.SetParent(roomRoot.transform);
        tvStand.transform.localScale = new Vector3(2, 0.5f, 0.6f);
        tvStand.transform.position = new Vector3(0, 0.25f, -3);
        tvStand.GetComponent<MeshRenderer>().material = woodMat;

        GameObject tvScreen = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tvScreen.name = "TV_Screen";
        tvScreen.transform.SetParent(roomRoot.transform);
        tvScreen.transform.localScale = new Vector3(1.8f, 1f, 0.1f);
        tvScreen.transform.position = new Vector3(0, 1.2f, -3);
        tvScreen.GetComponent<MeshRenderer>().material = tvMat;

        GameObject plantPot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        plantPot.name = "Plant_Pot";
        plantPot.transform.SetParent(roomRoot.transform);
        plantPot.transform.localScale = new Vector3(0.5f, 0.4f, 0.5f);
        plantPot.transform.position = new Vector3(-3, 0.4f, 4);
        plantPot.GetComponent<MeshRenderer>().material = floorMat;

        GameObject plantLeaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        plantLeaves.name = "Plant_Leaves";
        plantLeaves.transform.SetParent(roomRoot.transform);
        plantLeaves.transform.localScale = new Vector3(0.8f, 1.2f, 0.8f);
        plantLeaves.transform.position = new Vector3(-3, 1.2f, 4);
        plantLeaves.GetComponent<MeshRenderer>().material = plantMat;

        // 4. Lighting
        GameObject mainLight = new GameObject("MainWindowLight");
        mainLight.transform.SetParent(roomRoot.transform);
        Light dLight = mainLight.AddComponent<Light>();
        dLight.type = LightType.Directional;
        dLight.color = new Color(1f, 0.95f, 0.9f);
        dLight.intensity = 1.2f;
        dLight.shadows = LightShadows.Soft;
        mainLight.transform.rotation = Quaternion.Euler(30, -90, 0);

        GameObject lampLight = new GameObject("LampLight");
        lampLight.transform.SetParent(roomRoot.transform);
        lampLight.transform.position = new Vector3(-3, 2, 4);
        Light pLight = lampLight.AddComponent<Light>();
        pLight.type = LightType.Point;
        pLight.color = new Color(1f, 0.8f, 0.5f);
        pLight.intensity = 0.8f;
        pLight.range = 5f;

        // 5. Save as Prefab
        string prefabPath = "Assets/Environment/LivingRoom.prefab";
        PrefabUtility.SaveAsPrefabAssetAndConnect(roomRoot, prefabPath, InteractionMode.UserAction);

        Debug.Log("Living Room Environment successfully generated at: " + prefabPath);
    }
}
#endif
