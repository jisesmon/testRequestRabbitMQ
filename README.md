## testRequestRabbitMQ
in  proj
dotnet add package RabbitMQ.Client
dotnet add package Microsoft.Extensions.Hosting

## Docker 
docker pull rabbitmq:management 
docker run -d --hostname rmq --name rabbit-server2  -p 8080:15672 -p 5672:5672 rabbitmq:management  

--user:guest
--pass:guest
--role:admin

--user:ali 
--pass:ali
--role:monitoring

## Configs Rabbitmq server
<pre>
"users": [
		{
			"name": "ali",
		    "tags": [
				"management"        --!! -(management)+add and update  or  -(monitoring) only view data or -(none) that sent and revice to queue 
			],
			"limits": {
			}
		},
"vhosts": [
         {
			"name": "arpango.ir",
			"description": "",
			"metadata": {
				"description": "",
				"tags": [
				],
				"default_queue_type": "classic"
			},
			"tags": [
			]
		},
"permissions": [
		 {
			"user": "ali",
			"vhost": "arpango.ir",
			"configure": "arpango.ir*",  --!!! limit to view and and add and update
			"write": ".*",
			"read": ".*"
		},
</pre>
