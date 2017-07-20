# ACSMeetup.Alexa

Basic Intents in Alexa:
{
  "intents": [
    {
      "slots": [
        {
          "name": "question",
          "type": "AMAZON.LITERAL"
        }
      ],
      "intent": "QuestionIntent"
    },
    {
      "intent": "AMAZON.HelpIntent"
    },
    {
      "intent": "AMAZON.StopIntent"
    },
    {
      "intent": "AMAZON.CancelIntent"
    }
  ]
}
QuestionIntent to {read my sports news|question}
QuestionIntent {open questions|question}
