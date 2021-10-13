using UnityEngine;
using InputNM;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    private Interactable _collidedWith;

    private Inputstruct _interactInput = new Inputstruct(Input.GetKeyDown, KeyCode.Mouse0);

    private bool _inputEntered = false;

    private TextMeshProUGUI _interactableText;
    private Outline _outlineOfInteractable;

    [Zenject.Inject(Id = InjectIDs.INJECT_INTERACTABLE_TEXT)]
    GameObject interactableText;

    public void Construct(GameObject interactableText)
    {
        _interactableText = interactableText.GetComponent<TextMeshProUGUI>();
        _interactableText.gameObject.SetActive(false);
    }

    private void Awake()
    {

        _interactableText = interactableText.GetComponent<TextMeshProUGUI>();
        _interactableText.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (_collidedWith == null)
        {
            _interactableText.gameObject.SetActive(false);
            if (_outlineOfInteractable != null)
            {
                _outlineOfInteractable.enabled = false;
                _outlineOfInteractable = null;
            }
        }
        if (_interactInput.CheckInput())
        {
            _inputEntered = true;
        }
    }

    private void FixedUpdate()
    {
        if (_inputEntered)
        {
            _inputEntered = false;
            if (_collidedWith != null)
            {
                _collidedWith.Interact();
            }
        }
        _collidedWith = null;
    }

    private void OnTriggerStay(Collider other)
    {
        Interactable newCollidedWith = other.GetComponent<Interactable>();
        if (_collidedWith != newCollidedWith && newCollidedWith != null)
        {
            _collidedWith = newCollidedWith;
            _interactableText.text = newCollidedWith.TextOnScreen;
            _interactableText.gameObject.SetActive(true);
            _outlineOfInteractable = other.GetComponent<Outline>();
            if (_outlineOfInteractable != null)
            {
                _outlineOfInteractable.enabled = true;
            }
        }

    }
}
