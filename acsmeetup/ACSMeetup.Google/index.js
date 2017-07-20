// Copyright 2016, Google, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START app]
'use strict';

process.env.DEBUG = 'actions-on-google:*';

let ActionsSdkAssistant = require('actions-on-google').ActionsSdkAssistant;
let express = require('express');
let bodyParser = require('body-parser');
let api = require('sync-request');
let apiHost = "<Enter you API End Point URL>";
let currentUser = "googleHomeTestUser";

let app = express();
app.set('port', (process.env.PORT || 4093));
app.use(bodyParser.json({ type: 'application/json' }));

app.post('/', function (request, response) {
    console.log('handle post');
    const assistant = new ActionsSdkAssistant({ request: request, response: response });

    function mainIntent(assistant) {
        console.log('mainIntent');
        let speechText = getAnswer("welcome", assistant.getUser().user_id);
        let inputPrompt = assistant.buildInputPrompt(true, speechText,
            ['I didn\'t hear a question', 'If you\'re still there, what\'s ypur question?', 'Just ask What is the latest on followed by your search term']);
        assistant.ask(inputPrompt);
    }

    function rawInput(assistant) {
        console.log('rawInput');
        if (assistant.getRawInput() === 'bye') {
            assistant.tell('Leaving, so soon, oh why are guys always dumping me.');
        } else {
            let userRequest = assistant.getRawInput().toLowerCase();
            let speechText = getAnswer(userRequest, assistant.getUser().user_id);
            let inputPrompt = assistant.buildInputPrompt(true, speechText,
                ['I didn\'t hear a question', 'If you\'re still there, what\'s ypur question?', 'Just ask What is the latest on followed by your search term']);
            assistant.ask(inputPrompt);
        }
    }

    let actionMap = new Map();
    actionMap.set(assistant.StandardIntents.MAIN, mainIntent);
    actionMap.set(assistant.StandardIntents.TEXT, rawInput);

    assistant.handleRequest(actionMap);

    //Meetup API Functions
    function getAnswerSync(question, userId) {
        var querystring = require("querystring");
        var query = querystring.stringify({ q: question, device: "Google", user: userId });
        var apiUrl = "https://" + apiHost + "/bot/interact?" + query;
        console.log(apiUrl);
        var res = api('GET', apiUrl);
        var body = res.getBody();
        return JSON.parse(body);
    }

    function getAnswer(myquestion, userId) {
        let speechOutput = "";
        try {
            if (userId == null)
                userId = "unknown";
            let results = getAnswerSync(myquestion, userId);
            let repormptQuestion = "Anything Else?";
            return results.SSML;
        } catch (e) {
            return "<speak>Sorry, AudioBurst could play the selected item, please ask for anything else or just say, bye.</speak>";
        }
    }
});

// Start the server
let server = app.listen(app.get('port'), function () {
    console.log('App listening on port %s', server.address().port);
    console.log('Press Ctrl+C to quit.');
});
// [END app]
