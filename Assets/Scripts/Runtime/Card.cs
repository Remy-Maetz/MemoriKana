using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Card : MonoBehaviour
{
    public Matrix4x4 characterRects = Matrix4x4.zero;
    public Material sharedMaterial;
    private Material _material;
    public Material material
    {
        get
        {
            if (sharedMaterial == null)
                return null;

            if (_material == null)
                _material = Instantiate(sharedMaterial);

            return _material;
        }
    }

    private Image _image;
    public Image image
    {
        get
        {
            if (_image == null)
                _image = GetComponent<Image>();

            return _image;
        }
    }

    [Range(0f, 1f)]
    public float flipState = 0;

    private void OnValidate()
    {
        material.SetMatrix("_CharRects", characterRects);
        material.SetFloat("_FlipState", flipState);

        image.material = material;
    }
}
