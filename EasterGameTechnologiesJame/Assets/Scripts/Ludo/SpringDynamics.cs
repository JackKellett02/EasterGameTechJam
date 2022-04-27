using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used for menu and UI items.
/// It'll move the UI item towards a target position on the screen every frame based on the variables spring and drag.
/// It'll also contain some useful public functions for more responsive UI elements!
/// </summary>
public class SpringDynamics : MonoBehaviour
{
    
    #region Variables to assign via the unity inspector [SerializeFields]
    [SerializeField]
    [Range(0.5f,10.0f)]
    private float spring = 2f;
    [SerializeField]
    [Range(0.01f, 0.25f)]
    private float drag = 0.06f;

    [SerializeField]
    private Vector2 altPos = Vector2.zero;
    [SerializeField]
    private Vector2 offset = Vector2.zero;

    [SerializeField]
    private float altSizeMult = 0.0f;
    [SerializeField]
    private bool changesSize = true;
    #endregion

    #region Variable Declarations
    private RectTransform rect;
    private Vector2 primaryTarget;
    private Vector2 sizeTarget = Vector2.zero;
    private Vector2 altSizeTarget = Vector2.zero;

    private Vector2 velocity;
    private Vector2 sizeVelocity;
    private Vector2 internalOffset;
    #endregion

    #region Private Functions (Do not try to access from outside this class.)
    private void Start() {
        rect = GetComponent<RectTransform>();
        //The first target is always where the UI element is first placed.
        primaryTarget = rect.anchoredPosition;
        sizeTarget = rect.sizeDelta;
        altSizeTarget = rect.sizeDelta * altSizeMult;
    }

    /// <summary>
    /// In the update function, the UI element moves towards the primary target.
    /// the spring variable controls how fast the element moves.
    /// the drag variable controls how much bounce there is before the element sets into place, higher means less bouncing.
    /// For example, a high spring with a low drag will make a fast rumbling motion around the target before setting.
    /// </summary>
    private void FixedUpdate() {
        //Movement
        velocity += (primaryTarget + internalOffset - rect.anchoredPosition) * spring;
        velocity -= velocity * drag;
        rect.anchoredPosition += velocity * Time.deltaTime;

        //Size
        if (changesSize)
        {
            sizeVelocity += (sizeTarget - rect.sizeDelta) * spring;
            sizeVelocity -= sizeVelocity * drag;
            rect.sizeDelta += sizeVelocity * Time.deltaTime;
        }
        
    }
    #endregion

    #region Public Functions
    /// <summary>
    /// This function simply makes the UI element jump upwards based on the parameter entered by changing the velocity.
    /// </summary>
    /// <param name="force"></param>
    public void React(float force){
        velocity += Vector2.up * force * 250;
    }

    /// <summary>
    /// This function simply makes the UI element jump upwards based on the parameter entered by changing the velocity.
    /// </summary>
    /// <param name="force"></param>
    public void ReactRight(float force)
    {
        velocity += Vector2.right * force * 250;
    }

    /// <summary>
    /// Switches the current target position of a UI element with another set in the Unity Editor.
    /// Typically used for either toggles or switching between different menu containers.
    /// </summary>
    public void SwitchPos() {
        Vector2 temp = primaryTarget;
        primaryTarget = altPos;
        altPos = temp;
    }

    /// <summary>
    /// Teleports the UI object to its alternate position for use of UI objects coming into view after a scene transition
    /// </summary>
    public void AltPosTeleport()
    {
        rect.anchoredPosition = altPos;
    }

    public void SwitchSize(){
        Vector2 temp = sizeTarget;
        sizeTarget = altSizeTarget;
        altSizeTarget = temp;
    }

    ///<summary>
    ///Switch between offset values whenever mouse enters or leaves the UI element. Use an EventTrigger component for this!
    ///</summary>
    public void SwitchOffset()
    {
        Vector2 temp = offset;
        offset = internalOffset;
        internalOffset = temp;
    }

    public void AddXOffset(float offsetAdd)
    {
        internalOffset += new Vector2(offsetAdd,0);
    }
    #endregion

}
