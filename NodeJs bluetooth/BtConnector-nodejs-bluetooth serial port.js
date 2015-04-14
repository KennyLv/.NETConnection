// TODO disconnect events not implemented
// TODO refactor me (module, fire connection events, add slip support)

var btSerial = new (require('bluetooth-serial-port')).BluetoothSerialPort(),
    config = require('./config');

var PING = ' ',
    RECONNECT_TIMEOUT = 5000;

var getPairedDevices = function (callback) {
    btSerial.listPairedDevices(function (devices) {
        callback(devices);
    });
};

var getServiceByName = function (services, serviceName) {
    return services.filter(function (service) {
        return service.name === serviceName;
    })[0];
};

var getDeviceByServiceName = function (devices, serviceName) {
    return devices.filter(function (device) {
        return getServiceByName(device.services, serviceName);
    })[0];
};

var getHapDevice = (function () {

    var isOSX = 'darwin';

    if (process.platform === isOSX) {

        return function (callback) {
            getPairedDevices(function (devices) {
                var device = getDeviceByServiceName(devices, config.SDP_NAME);

                if (device) {
                    callback({
                        address: device.address,
                        channel: getServiceByName(device.services, config.SDP_NAME).channel
                    });
                } else {
                    console.log('No devices found with the service name: ', config.SDP_NAME);
                    console.log('Try to pair/unpair your devices');
                    console.log('Before pairing HAP should be running');
                }

            });
        };

    }

    return function (callback) {
        btSerial.findSerialPortChannel(config.DEVICE_BT_ADDRESS, function (channel) {
            callback({
                address: config.DEVICE_BT_ADDRESS,
                channel: channel
            });
        }, function () {
            console.error('Unable to find SerialPortChannel');
            callback();
        });
    };
})();

function connect () {

    console.log('Searching for device...');

    btSerial.close();

    getHapDevice(function (device) {

        console.log('Connecting... Channel ', device ? device.channel : ' not found');

        if (device) {

            btSerial.connect(device.address, device.channel, function() {

                console.log('Connected');

                btSerial.on('data', function(buffer) {
                    console.log('data: ', buffer.toString('utf-8'));
                });

                btSerial.on('failure', function(err) {
                    console.log('failure: ', err);
                    setTimeout(connect, RECONNECT_TIMEOUT);
                });

                // init session
                btSerial.write(new Buffer(PING, 'utf-8'), function(err) {
                    if (err) {
                        console.log('write err: ', err);
                        setTimeout(connect, RECONNECT_TIMEOUT);
                    }
                });

            }, function () {
                setTimeout(connect, RECONNECT_TIMEOUT);
            });

            btSerial.close();

        } else {
            setTimeout(connect, RECONNECT_TIMEOUT);
        }
    });
}

exports.connect = connect;