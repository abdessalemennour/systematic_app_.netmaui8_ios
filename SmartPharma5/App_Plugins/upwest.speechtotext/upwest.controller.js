(function () {
    'use strict';
    function speechToTextController($http, $scope, $routeParams) {

        var vm = this;
        vm.alias = $scope.model.alias;

        speechRecognition.lang = "en-US"

        setLanguage();
        tinymce.on('AddEditor', function (e) {
          e.editor.on('focus', function (ev) {
            isInput = false;
              final_transcript = e.editor.getContent() + " ";
          });

          e.editor.on('keydown', function (ev) {
              final_transcript = e.editor.getContent();
          });
        });

        setTimeout(function () {

            $("input,textarea").focus(function () {
                focused = this;
                final_transcript = $(this).val() + " ";
                isInput = true;
            });

            $("input,textarea").focusout(function () {
                isInput = false;
            });

            $("input,textarea").on("keyup keydown", function (event) {
                final_transcript = $(this).val();
            });

        }, 1000);

        $("#startMic").on("click", function (event) {
            if (!isRecording) {
                setLanguage();
                $("#startMic").attr("src", "/App_Plugins/upwest.speechtotext/images/mic-animate.gif");
                speechRecognition.start();
            }
            else {
                $("#startMic").attr("src", "/App_Plugins/upwest.speechtotext/images/mic.gif");
                speechRecognition.stop();
            }
        });

        function setLanguage() {
            if ($routeParams.cculture != null) {
                speechRecognition.lang = $routeParams.cculture;
            }

            if ($routeParams.mculture != null) {
                speechRecognition.lang = $routeParams.mculture;
            }
        }
    }

    angular.module('umbraco').controller('upwest.speechtotext', speechToTextController);

})();











