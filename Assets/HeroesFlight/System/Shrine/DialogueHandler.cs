using System.Collections;
using UnityEngine;

namespace HeroesFlight.System.Shrine
{
    public class DialogueHandler
    {
        public DialogueHandler(Canvas chatCanvas, MonoBehaviour owner, float conversationDuration)
        {
            this.chatCanvas = chatCanvas;
            this.owner = owner;
            this.conversationDuration = conversationDuration;
        }

        private Canvas chatCanvas;
        private MonoBehaviour owner;
        private float conversationDuration;

        private bool isInConversation;


        public void TryTriggerConversation()
        {
            if (isInConversation)
                return;


            isInConversation = true;
            owner.StartCoroutine(ConversationRoutine());
        }


        IEnumerator ConversationRoutine()
        {
            chatCanvas.enabled = true;
            yield return new WaitForSeconds(conversationDuration);
            chatCanvas.enabled = false;
            isInConversation = false;
        }
    }
}