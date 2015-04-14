var exec = require('child_process').exec,
    spawn = require('child_process').spawn,
    localAddress = "192.168.0.2",
    destinationAddress = "192.168.0.3",
    deviceAddr = require('./config.js').DEVICE_BT_ADDRESS,
    Bluetooth = function(){};

//Scan for all Bluetooth devices
Bluetooth.prototype.scan = function(device, callback){
    // Now try to scan for Bluetooth devices
    // If we find the last connected device we auto connect to it
    console.log("Scanning for bluetooth Devices ....");
    getMipChannel(device, callback);
};
//connect to specified device address
Bluetooth.prototype.connect = function(device,callback){
    // Now try to scan for Bluetooth devices
    // If we find the last connected device we auto connect to it
    rfcommReleaseAndReconnect(device,callback);
};


var configureNetwork = function(device,slVal,callback) {
    console.log("Configuring Network");
    // remove an old rfcomm binding, if it exists
    var execIfconfig = "ifconfig " + slVal + " " + localAddress + " dstaddr " + destinationAddress;
    console.log('>', execIfconfig);
    this.child = exec(execIfconfig, function (error) {
            if (error !== null) {
                callback(error,false);
                console.log('exec error: ' + error);
            }else{
                console.log("CONNECTED TO "+ device.name);
                callback(null,true);
            }
        }
    );
};

//Attaches the local RFCOMM device to the network.
var attachNetwork = function(device,callback) {
    console.log("Calling slattach ");
    // remove an old rfcomm binding, if it exists
    var child = spawn('slattach', ['-d', '-p', 'slip', '/dev/rfcomm0']);
    child.stdout.on('data', function (data) {
        console.log("slattach data "+data);
        var stdoutdata = data + "";
        var slPtrn = /sl(\d)/;
        var matches = stdoutdata.match(slPtrn);
        if(matches && matches.length > 0){
            console.log(matches[0]);
            configureNetwork(device,matches[0],callback);
        }
    });

    child.stderr.on('data', function (data) {
        console.log('stderr: ' + data);
        console.log('Did you run it as a super user?');
    });

    child.on('exit', function (code) {
        console.log('child process exited with code ' + code);
    });

};


var rfcommBind = function(device,callback) {
    // bind the remote service to a local rfcomm device
    console.log("Binding to rfcomm \n");
    var execBind = "rfcomm bind /dev/rfcomm/0 " + device.address + " " + device.channel;
    console.log('>', execBind);
    this.child = exec(execBind, function (error) {
            console.log("rfcomm bind");
            if (error !== null) {
                console.log('exec error: ' + error);
                callback(error,null);
            } else {
                attachNetwork(device,callback);
            }
        });
};


//Releases the RFCOMM binding
var rfcommReleaseAndReconnect = function(device,callback) {
    // remove an old rfcomm binding, if it exists
    console.log("Releasing rfcomm");
    var execReleaseRfcomm = "rfcomm release /dev/rfcomm/0";
    console.log('>', execReleaseRfcomm);
    exec(execReleaseRfcomm, function () {
        console.log("rfcomm release");
        rfcommBind(device,callback);
    });
};

var getMipChannel = function(device,callNext) {
    console.log("Trying to get mip Channel on "+ device);
    var cmd = "sdptool browse " + device;
    console.log('>', cmd);
    exec(cmd,
        function (error, stdout, stderr) {
            console.log("Std error  " + stderr);
            // console.log("std out  " + stdout);
            if (error !== null) {
                //console.log('exec error: ' + error);
                callNext(null);
            }else{
                //console.log("stdout ****** "+ stdout);
                var channelmatchPtrn = new RegExp("Service Name: MIP[\\s[\\s\\S]*?Channel: (\\d*)[\\s\\S]*");
                var matches =  stdout.match(channelmatchPtrn);
                // console.log("std out >>>>> " + matches);
                if(matches && matches.length > 0){
                    var matchedChannel = parseInt(matches[1],10);
                    console.log("Found MIP enabled Device on Channel " + matchedChannel);
                    var deviceData = {
                        name : device,
                        address : device,
                        channel : matchedChannel
                    };
                    callNext(null, [deviceData]);
                } else {
                    console.log("Unable to find MIP service");
                    callNext(null);
                }
            }

        }
        );
};

exports.connect = function(){

    var bt = new Bluetooth();

    bt.scan(deviceAddr, function(err, devfound){
        if(devfound && devfound.length > 0){
            console.log("Found these devices "+ devfound[0].address);
            bt.connect(devfound[0], function(err, status){
                console.log("Connected to HAP: "+ status);
            });
        } else {
            console.log("Scan devices error: " + err);
        }

    });

};