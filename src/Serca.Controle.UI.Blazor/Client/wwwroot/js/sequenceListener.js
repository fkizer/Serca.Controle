﻿// Source : https://github.com/kegi/sequence-listener/
// Commmit : 043ded4682bdfc098ca6f9b57d71ca827180be8b
// Modifications made by Serca :
//  -   Adding prop override
//  -   Adding prop preventEnter
//  -   Adding prop overrideTargetButtons

'use strict';

var isSequenceListenerInitialized = false;

/**
 * @class SequenceListener
 * @param {Object} configs
 * @constructor
 */
var SequenceListener = function (configs) {

    this.initialize(configs);
};

/**
 * @method initialize
 * @param {Object} configs
 */
SequenceListener.prototype.initialize = function (configs) {

    if (isSequenceListenerInitialized) {
        return;
    }

    isSequenceListenerInitialized = true;

    this.configs = this.extendConfig({

        debug: false,
        maxKeyboardDelay: 75,
        minLength: 5,
        exactLength: null,
        allowedChars: '[a-zA-Z0-9]',
        ignoreInputs: true,
        preventEnter: false,
        overrideTargetButtons: false

    }, configs);

    this.validateConfigs();

    this.sequence = '';
    this.lastCharRecordedTime = null;
    this.targetElement = null;
    this.watcher = null;

    this.listenKeyboard();
};

/**
 * @method validateConfigs
 */
SequenceListener.prototype.validateConfigs = function () {

    if (this.configs.exactLength === null && this.configs.minLength === null) {
        throw 'You need to specify "exactNbChars" or "minLength"';
    }

    if (this.configs.maxKeyboardDelay < 10 || this.configs.maxKeyboardDelay > 2000) {
        throw '"maxKeyboardDelay" need to be between 10 and 2000';
    }
};

/**
 * @method extendConfig
 * @param {Object} defaultConfigs
 * @param {Object} configs
 * @returns {Object}
 */
SequenceListener.prototype.extendConfig = function (defaultConfigs, configs) {

    for (var key in configs) {
        if (configs.hasOwnProperty(key)) {
            defaultConfigs[key] = configs[key];
        }
    }

    return defaultConfigs;
};

/**
 * @method listenKeyboard
 */
SequenceListener.prototype.listenKeyboard = function () {

    document.onkeyup = function (keyEvent) {
        this.keyUp(keyEvent);
    }.bind(this);

    // Added by Serca
    document.onkeydown = function (keyEvent) {
        this.keyDown(keyEvent);
    }.bind(this);
};

/**
 * @method keyUp
 * @param {Object} keyEvent
 */
SequenceListener.prototype.keyDown = function (keyEvent) {

    this.preventEnter(keyEvent || window.event);
};

/**
 * @method keyUp
 * @param {Object} keyEvent
 */
SequenceListener.prototype.keyUp = function (keyEvent) {

    this.recordKey(keyEvent || window.event);
};

/**
 * Added by Serca
 * @method recordKey
 * @param {Object} keyEvent
 */
SequenceListener.prototype.preventEnter = function (keyEvent) {

    if (!this.configs.preventEnter || this.sequence.length === 0) {
        return;
    }

    var charCode = keyEvent.charCode || keyEvent.keyCode;

    if (charCode != 13) {
        return;
    }

    keyEvent.preventDefault();
};

/**
 * @method recordKey
 * @param {Object} keyEvent
 */
SequenceListener.prototype.recordKey = function (keyEvent) {

    if (this.configs.ignoreInputs) {
        if (this.isInputTarget(keyEvent)) {

            this.clean(true);
            this.clearWatcher();

            return;
        }
    }

    if (this.override) {

        // Edited by Serca
        var charCode = keyEvent.charCode || keyEvent.keyCode;
        var character = keyEvent.key;

        if (character.length == 1) {
            var ascii = character.charCodeAt(0);

            if (this.configs.debug) {
                console.log('Code ASCII: ' + ascii);
            }

            if ((ascii < 48 || ascii > 57) && (ascii < 65 || ascii > 90) && (ascii < 97 || ascii > 122)) {
                character = String.fromCharCode(charCode);
            }

            if (this.configs.debug) {
                console.log('Character: ' + character);
            }
        }
    }
    else {
        // Original :
        var charCode = keyEvent.charCode || keyEvent.keyCode,
            character = keyEvent.key || String.fromCharCode(charCode);
    }

    if (character.length > 1 || !this.checkValidChar(character)) {
        return;
    }

    this.clean(false);
    this.clearWatcher();

    this.lastCharRecordedTime = new Date().getTime();
    this.sequence += character;

    if (this.sequence.length === 1) {
        this.targetElement = keyEvent.target;
    }

    if (this.configs.debug) {
        console.log(this.sequence);
    }

    this.setWatcher();
};

/**
 * @method isInputTarget
 * @param {Object} event
 * @returns {boolean}
 */
SequenceListener.prototype.isInputTarget = function (event) {

    switch (event.target.tagName.toLowerCase()) {

        case 'input':
        case 'textarea':

            return true;

        default:

            return false;
    }
};

/**
 * @method checkValidChar
 * @param {string} input
 * @returns {boolean}
 */
SequenceListener.prototype.checkValidChar = function (input) {

    return input.match(new RegExp(this.configs.allowedChars)) !== null;
};

/**
 * @method clean
 * @param {boolean} force
 */
SequenceListener.prototype.clean = function (force) {

    if (force === undefined) {
        force = false;
    }

    if (force || !this.checkDelay()) {

        if (this.configs.debug) {
            console.log('-- CLEARED --');
        }

        this.sequence = '';
    }
};

/**
 * @method checkDelay
 * @returns {boolean}
 */
SequenceListener.prototype.checkDelay = function () {

    return this.lastCharRecordedTime === null ||
        (new Date().getTime() - this.lastCharRecordedTime) <= this.configs.maxKeyboardDelay;
};

/**
 * @method setWatcher
 */
SequenceListener.prototype.setWatcher = function () {

    this.watcher = setTimeout(function () {
        this.watcher = null;
        this.checkCompleted();
    }.bind(this), this.configs.maxKeyboardDelay);
};

/**
 * @method clearWatcher
 */
SequenceListener.prototype.clearWatcher = function () {

    if (this.watcher !== null) {
        clearTimeout(this.watcher);
        this.watcher = null;
    }
};

/**
 * @method isRecordedInputValid
 * @param {string} input
 * @returns {boolean}
 */
SequenceListener.prototype.isRecordedInputValid = function (input) {

    return (this.configs.exactLength !== null && input.length === this.configs.exactLength) ||
        input.length >= this.configs.minLength;
};

/**
 * @method checkCompleted
 */
SequenceListener.prototype.checkCompleted = function () {

    var sequence = this.sequence;

    if (this.isRecordedInputValid(sequence)) {
        this.dispatchEvent(sequence);
    } else {
        this.clean(false);
    }
};

/**
 * @method newInputDetected
 * @param {string} sequence
 */
SequenceListener.prototype.dispatchEvent = function (sequence) {

    if (this.configs.debug) {
        console.log('=========================');
        console.log('detected : ' + sequence);
        console.log('=========================');
    }

    var target = this.targetElement;

    if (target.tagName.toLowerCase() === "button" && this.configs.overrideTargetButtons) {
        target = document.body;
    }

    target.dispatchEvent(new CustomEvent(
        'keyboardSequence', {
        detail: {
            sequence: sequence
        }
    }));

    this.clean(true);
};

if (typeof define === "function" && define.amd) {
    define(function () {

        if (typeof window.SequenceListener !== 'undefined') {
            return window.SequenceListener;
        }

        return window.SequenceListener = SequenceListener;
    });
}