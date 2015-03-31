<?php

namespace WebSocket\Application;

/**
 * Example WebSocket Application
 * @author Dmitru Gulyakevich
 */
class ExampleApplication extends Application
{

//    public function onConnect($client)
//    {
//        parent::onConnect($client);
//
//        $client->broadcastServer('Join!');
//    }
//
//    public function onDisconnect($client)
//    {
//        $client->broadcastServer('Bye-bye!');
//
//        parent::onDisconnect($client);
//    }
//
//    public function onTick()
//    {
//        $this->sendApp('Approximately every second second within the application.');
//    }

    public function onData($jData, $client)
    {
        $data = json_decode($jData);

        switch (isset($data->type) ? $data->type : null) {

            // add to group or disconnect
            case 'join':

                isset($data->group_id) ?
                                $client->setGroup($data->group_id) :
                                $client->onDisconnect();
                return;
                break;
            // add case 'login';
            case 'login':
            	isset( $data->name )?
            	        $client->setName( $data->name ):$client->onDisconnect();
            	return;
                break;        
            case 'close':
                $client->onDisconnect();
                return;
                break;
            case 'logout':
            	$client->logout();
            	return;
                break;  
            case 'getUsers':
            	$client->getUsers();
            	return ;
            	break;   
            case 'forward':
            	$client->forwardData( $data->name , $data->msg );
            	return ;
            	break; 
            case 'updateState':
            	$client->updateState( $data->state );
            	return ;
            	break;
            default:
                break;
        }

//        to current socket
//        $client->send($data->comment);
//
//        to all server sockets
        $client->sendServer($data->comment);

//        to all server sockets exclude sender
//        $client->broadcastServer($data->comment);
//
//        Group
//
//        $client->sendGroup($data->comment);
//        $client->broadcastGroup($data->comment);
//        $client->getConnectionsByGroupKey(1);
//
//        Application
//
//        $this->sendApp($data->comment);
//        $this->broadcastApp($client, $data->comment);
    }

}