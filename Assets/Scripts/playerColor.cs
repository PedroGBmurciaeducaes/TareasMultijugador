using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerColor : MonoBehaviourPunCallbacks
{
    private Renderer myRenderer;
    private MaterialPropertyBlock propBlock;

    private static readonly int ColorID = Shader.PropertyToID("_Color");
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    void Awake()
    {
        myRenderer = GetComponentInChildren<Renderer>(true);

        if (myRenderer != null)
        {
            // Si NO hay material asignado en el prefab, crea uno en runtime
            if (myRenderer.sharedMaterial == null)
            {
                var urp = Shader.Find("Universal Render Pipeline/Lit");
                var std = Shader.Find("Standard");
                var shader = urp != null ? urp : std;           // soporte URP/Built-in
                myRenderer.sharedMaterial = new Material(shader);
            }

            // Si quieres seguir clonando el material (opcional):
            // myRenderer.material = new Material(myRenderer.sharedMaterial);
        }

        propBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        if (photonView.IsMine)
            ChangeColor();
    }

    public void ChangeColor()
    {
        StartCoroutine(ChangeColorAfterDelay());
    }

    IEnumerator ChangeColorAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Color newColor = new Color(Random.value, Random.value, Random.value);
        photonView.RPC(nameof(UpdateColor), RpcTarget.AllBuffered, newColor.r, newColor.g, newColor.b);
    }

    [PunRPC]
    public void UpdateColor(float r, float g, float b)   // <- PUBLIC
    {
        ApplyColor(new Color(r, g, b));
    }

    void ApplyColor(Color color)
    {
        if (myRenderer == null) { Debug.LogError("Renderer not found on " + gameObject.name); return; }

        myRenderer.material.color = color;
        //var mat = myRenderer.sharedMaterial;
        //if (mat == null)
        //{
        //    Debug.LogError("Asigna un Material al MeshRenderer del prefab (ahora está 'None').");
        //    return;
        //}


        //myRenderer.GetPropertyBlock(propBlock);

        //if (mat.HasProperty(BaseColorID))       // URP/HDRP Lit
        //    propBlock.SetColor(BaseColorID, color);
        //else if (mat.HasProperty(ColorID))      // Built-in Standard
        //    propBlock.SetColor(ColorID, color);
        //else
        //    myRenderer.material.color = color;  // último recurso

        //myRenderer.SetPropertyBlock(propBlock);
    }
}