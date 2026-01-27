using Module.Gameplay;
using TMPro;
using UnityEngine;
using Zenject;

namespace Module.Presentation.UI
{
    public sealed class MessageView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageTextArea;

        private MessageService _service;

        [Inject]
        public void Construct(MessageService service) => _service = service;

        private void OnEnable() => _service.OnTextChanged += HandleTextChanged;
        private void Start() => _service.InitializeForNewDay();
        private void HandleTextChanged(string text) => _messageTextArea.text = text;
        private void OnDisable() => _service.OnTextChanged -= HandleTextChanged;
    }
}