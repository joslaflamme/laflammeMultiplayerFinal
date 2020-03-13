var io = require('socket.io')(process.env.PORT || 3000);
var shortid = require("shortid");
var mongoose = require('mongoose');
var db = require("./database");

//console.log(shortid.generate());
require("./Player");
var Player = mongoose.model("players");



mongoose.connect(db.mongoURI,{
    useNewUrlParser:true,
    useUnifiedTopology: true
}).then(function(){
    console.log("mongo db connected");
}).catch(function(err){
    console.log(err);
});

console.log('Server connected');
var players = [];

io.on('connection', function(socket){
    console.log('client connected');

    var thisClientId = shortid.generate();
    players.push(thisClientId);
    var newPlayer = {
        id:thisClientId,
        score:100,
        name:thisClientId
    }
    Player(newPlayer).save().then(function(players){

    })
    //spawn all newly joined players
    socket.broadcast.emit('spawn', {id:thisClientId});
    //request logged in player's position
    socket.broadcast.emit('requestPosition');
   

    players.forEach(function(playerId){

        if(playerId == thisClientId){
            return;
        }

        socket.emit('spawn', {id:playerId});
        console.log('spawning player of id: ', playerId);
    });
    
    socket.on('yolo', function(data){
        console.log('You only yolo yolo');
        console.log(data);
    });
    socket.on('updateScore', function(data){
        data.id = thisClientId;
        socket.broadcast.emit('updateScore', data);
    });
    socket.on('updatePosition', function(data){
        data.id = thisClientId;
        socket.broadcast.emit('updatePosition', data);
    });

    socket.on('move', function(data){
        data.id = thisClientId;
        console.log("player is moving", JSON.stringify(data));
        socket.broadcast.emit('move', data);
    });

    socket.on('disconnect',function(){
        console.log("player disconnected");
       players.splice(players.lastIndexOf(thisClientId), 1);
       socket.broadcast.emit('disconnected', {id:thisClientId});

    })
});

