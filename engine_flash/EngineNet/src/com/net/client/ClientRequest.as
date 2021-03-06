package com.net.client
{
	import com.cell.net.io.MutualMessage;
	import com.cell.util.Reference;
	
	import flash.utils.getTimer;
	
	internal class ClientRequest extends Reference
	{
		private var request 		: MutualMessage;
		private var package_num		: int;
		private var drop_timeout	: int;
		
		private var fresponse 		: Array;
		private var ftimeout 		: Array;
		
		private var sent_time		: int;
		private var ping			: int;
		
		function ClientRequest(
			message 		: MutualMessage, 
			package_num		: int,
			drop_timeout 	: int, 
			response 		: Array,
			timeout 		: Array
		){
			this.request 		= message;
			this.package_num 	= package_num;
			this.drop_timeout	= drop_timeout;
			
			this.fresponse 		= response;
			this.ftimeout 		= timeout;
		}
		
		internal function getPacketNumber() : int
		{
			return this.package_num;
		}
		
		internal function send(client : Client) : void
		{
			this.sent_time = getTimer();
			client.getSession().sendRequest(package_num, this.request);
		}
		
		internal function getRequest() : MutualMessage
		{
			return request;
		}
		
		internal function isTimeout() : Boolean {
			return (getTimer() - this.sent_time) > drop_timeout;
		}
		
		internal function onResponse(client:Client, protocol : Protocol) : void
		{
			this.ping = getTimer() - sent_time;
			this.set(protocol.getMessage());
			if (fresponse != null) {
				var event : ClientEvent = new ClientEvent(ClientEvent.MESSAGE_RESPONSE, client, 
					protocol, this.request, null);
				for each (var response : Function in this.fresponse) {
					try {
						response.call(response, event);
					} catch (err:Error) {
						trace(err.message, err);
					}
				}
			}
		}
		
		internal function onTimeout(client:Client) : void
		{
			if (ftimeout != null) {
				var event : ClientEvent = new ClientEvent(ClientEvent.REQUEST_TIMEOUT, client, 
					null, this.request, null);
				for each (var timeout : Function in this.ftimeout) {
					try {
						timeout.call(timeout, event);
					} catch (err:Error) {
						trace(err.message, err);
					}
				}
			}
		}
	}
}