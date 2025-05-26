var focused = null;
let final_transcript = "";
var isInput = false;
var isRecording = false;
var SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition || window.msSpeechRecognition;
var speechRecognition = SpeechRecognition ? new SpeechRecognition() : null;

if (speechRecognition) {
    speechRecognition.continuous = true;
    speechRecognition.interimResults = true;

    speechRecognition.onstart = () => {
        isRecording = true;
        $("#startMic").attr("src", "/App_Plugins/upwest.speechtotext/images/mic-animate.gif");
    };

    speechRecognition.onerror = (event) => {
        $("#startMic").attr("src", "/App_Plugins/upwest.speechtotext/images/mic-slash.gif");
        isRecording = false;

        console.log(event.error);

        if (event.error === "not-allowed") {
            alert("To enable microphone detection, please remove the 'microphone=()' value from the 'Permissions-Policy' headers and check browser mic authorization for your website!");
        } else {
            console.log(event.error);
        }
    };

    speechRecognition.onend = () => {
        final_transcript = "";
        isRecording = false;
        $("#startMic").attr("src", "/App_Plugins/upwest.speechtotext/images/mic.gif");
    };

    speechRecognition.onresult = (event) => {
        let interim_transcript = "";

        for (let i = event.resultIndex; i < event.results.length; ++i) {
            if (event.results[i].isFinal) {
                final_transcript += event.results[i][0].transcript;

                if (!isInput) {
                    tinymce.activeEditor.setContent(final_transcript);
                } else {
                    $(focused).val(final_transcript);
                    const scope = angular.element(focused).scope();
                    if (scope) {
                        scope.$apply(() => {
                            scope.$eval($(focused).attr('ng-model') + " = '" + final_transcript + "'");
                        });
                    }
                }
            } else {
                interim_transcript += event.results[i][0].transcript;

                if (!isInput) {
                    tinymce.activeEditor.setContent(final_transcript + interim_transcript);
                } else {
                    $(focused).val(final_transcript + interim_transcript);
                    const scope = angular.element(focused).scope();
                    if (scope) {
                        scope.$apply(() => {
                            scope.$eval($(focused).attr('ng-model') + " = '" + (final_transcript + interim_transcript) + "'");
                        });
                    }
                }
            }
        }
    };
} else {
    $("#startMic").attr("src", "/App_Plugins/upwest.speechtotext/images/mic-slash.gif");
    alert("Speech recognition is not supported in this browser. Please use a compatible browser like Chrome, Edge, or Firefox.");
}