/// <reference path="jquery-2.0.3.intellisense.js" />

var conn;

function openConnection() {
    if (conn.readyState === undefined|| conn.readyState > 1) {
        
        conn = new WebSocket('ws://localhost:8100');

        conn.onopen = function () {
            var userName = document.getElementById("txtUsername");
            conn.send("uc:" + userName.value);
        };

        conn.onmessage = function (event) {
            var d = document.getElementById("content");
            d.innerHTML = event.data + "<br />" + d.innerHTML;
        };

        conn.onerror = function () {
            alert("WebSocket Error");
        };

        conn.onclose = function () {
            alert("WebSocket Closed");
        };
    }
}

$(function () {
    $('#btnConnect').click(function () {
        conn = {}, window.WebSocket = window.WebSocket || window.MozWebSocket
        openConnection();
        $('#usernameRequest').hide('slow');
    });

    function sendMessage() {
        if (conn != undefined) {
            var mess = $('#txtMessage').val();
            conn.send(mess);
            $('#txtMessage').val('');
            $('#txtMessage').focus();
        } else {
            alert("Yiu are not connected");
        }
    }

    $('#btnSend').click(function () {
        sendMessage();
    });

    $('#txtMessage').keyup(function (event) {
        var key = event.keyCode || event.which;
        if (key === 13) {
            sendMessage();
        }
        return false;
    });
});