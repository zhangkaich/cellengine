package com.net.flash.test.Messages
{
	import com.cell.net.io.MutualMessage;
	import com.cell.net.io.MessageFactory;
	import com.cell.net.io.NetDataTypes;
	import com.cell.util.Map;


	/**
	 * 
	 * Java Class [5] [com.net.flash.test.Messages.EchoRequest]<br>
	 * 此代码为自动生成。不需要在此修改。若有错误，请修改代码生成器。
	 */
	[Bindable] 
	public  class EchoRequest  implements MutualMessage
	{
		/** Java type is : <font color=#0000ff>java.lang.String</font>*/
		public var message : String;

		/** Java type is : <font color=#0000ff>com.net.flash.test.Messages.Data</font>*/
		public var data : com.net.flash.test.Messages.Data;

		/** Java type is : <font color=#0000ff>com.net.flash.test.Messages.Data[]</font>*/
		public var datas : Array;

		/** Java type is : <font color=#0000ff>com.net.flash.test.Messages.Data[][][]</font>*/
		public var datas2 : Array;



		/**
		 * @param message as <font color=#0000ff>java.lang.String</font>
		 * @param data as <font color=#0000ff>com.net.flash.test.Messages.Data</font>
		 * @param datas as <font color=#0000ff>com.net.flash.test.Messages.Data[]</font>
		 * @param datas2 as <font color=#0000ff>com.net.flash.test.Messages.Data[][][]</font>		 */
		public function EchoRequest(
			message : String = null,
			data : com.net.flash.test.Messages.Data = null,
			datas : Array = null,
			datas2 : Array = null) 
		{
			this.message = message;
			this.data = data;
			this.datas = datas;
			this.datas2 = datas2;
		}
		


	}
}