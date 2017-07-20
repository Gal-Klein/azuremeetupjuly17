var apiHost = "<Enter you API End Point URL>";
var https = require('http');
var request = require('sync-request');
var querystring = require("querystring");

// Route the incoming request based on type (LaunchRequest, IntentRequest,
// etc.) The JSON body of the request is provided in the event parameter.
exports.handler = function (event, context) {
    try {
        /**
         * Uncomment this if statement and populate with your skill's application ID to
         * prevent someone else from configuring a skill that sends requests to this function.
         */

        //if (event.session.application.applicationId !== "amzn1.ask.skill.26052e1b-6021-44a4-9d19-0cf18dd1fd8c") {
        //     context.fail("Invalid Application ID");
        //}


        if (event.session.new) {
            onSessionStarted({ requestId: event.request.requestId }, event.session);
        }

        if (event.request.type === "LaunchRequest") {
            onLaunch(event.request,
                event.session,
                function callback(sessionAttributes, speechletResponse) {
                    context.succeed(buildResponse(sessionAttributes, speechletResponse));
                });
        } else if (event.request.type === "IntentRequest") {
            onIntent(event.request,
                event.session,
                function callback(sessionAttributes, speechletResponse) {
                    context.succeed(buildResponse(sessionAttributes, speechletResponse));
                });
        } else if (event.request.type === "SessionEndedRequest") {
            onSessionEnded(event.request, event.session);
            context.succeed();
        }
    } catch (e) {
        console.log(e.message);
        context.fail("Exception: " + e);
    }
};

//Called when the session starts.
function onSessionStarted(sessionStartedRequest, session) {
    console.log("onSessionStarted requestId=" + sessionStartedRequest.requestId +
        ", sessionId=" + session.sessionId);
}

//Called when the user launches the skill without specifying what they want.
function onLaunch(launchRequest, session, callback) {
    console.log("onLaunch requestId=" + launchRequest.requestId +
        ", sessionId=" + session.sessionId);
    // Dispatch to your skill's launch.
    getAnswer(null, "welcome", callback, session);
}

//Called when the user specifies an intent for this skill.
function onIntent(intentRequest, session, callback) {
    console.log("onIntent requestId=" + intentRequest.requestId +
        ", sessionId=" + session.sessionId);

    var intent = intentRequest.intent,
        intentName = intentRequest.intent.name;
    // Dispatch to your skill's intent handlers
    if ("AMAZON.HelpIntent" === intentName) {
        getAnswer(intent, "help", callback, session);
    } else if ("AMAZON.StopIntent" === intentName || "AMAZON.CancelIntent" === intentName || "StopIntent" === intentName) {
        handleSessionEndRequest(callback);
    } else if ("QuestionIntent" === intentName) {
        getAnswer(intent, intent.slots.question.value, callback, session);
    } else {
        throw "Invalid intent";
    }
}
/**
 * Called when the user ends the session.
 * Is not called when the skill returns shouldEndSession=true.
 */
function onSessionEnded(sessionEndedRequest, session) {
    console.log("onSessionEnded requestId=" + sessionEndedRequest.requestId +
        ", sessionId=" + session.sessionId);
    // Add cleanup logic here
}

// --------------- Functions that control the skill's behavior -----------------------

function getAnswer(intent, question, callback, session) {
    var userId = session.user.userId;
    var query = querystring.stringify({ userId: userId, device: "alexa", q: question });
    var response = getResponse("/bot/interact?" + query);
    var cardTitle = "Here are your Results for: " + question;
    var speechOutput = response.SSML;
    var repromptText = "Anything Else? If not just say stop.";
    var shouldEndSession = false;
    var sessionAttributes = [];
    callback(sessionAttributes,
        buildSSMLSpeechletResponse(cardTitle, speechOutput, repromptText, shouldEndSession));
    return;
}

function getResponse(uri) {
    var querystring = require("querystring");
    var apiUrl = "https://" + apiHost + uri;
    console.log("audioburst debug: getResponse=" + apiUrl);
    var res = request('GET', apiUrl);
    var body = res.getBody();
    return JSON.parse(body);
};

function handleSessionEndRequest(callback) {
    var cardTitle = "Session Ended";
    var speechOutput = "Leaving, so soon, oh why are guys always dumping me...";
    var shouldEndSession = true;
    callback({}, buildSpeechletResponse(cardTitle, speechOutput, null, shouldEndSession));
}

// --------------- Helpers that build all of the responses -----------------------

function buildSpeechletResponse(title, output, repromptText, shouldEndSession) {
    return {
        outputSpeech: {
            type: "PlainText",
            text: output
        },
        reprompt: {
            outputSpeech: {
                type: "PlainText",
                text: repromptText
            }
        },
        shouldEndSession: shouldEndSession
    };
}

function buildSSMLSpeechletResponse(title, output, repromptText, shouldEndSession) {
    return {
        outputSpeech: {
            type: "SSML",
            ssml: output
        },
        reprompt: {
            outputSpeech: {
                type: "PlainText",
                text: repromptText
            }
        },
        shouldEndSession: shouldEndSession
    };
}

function buildResponse(sessionAttributes, speechletResponse) {
    return {
        version: "1.0",
        sessionAttributes: sessionAttributes,
        response: speechletResponse
    };
}
