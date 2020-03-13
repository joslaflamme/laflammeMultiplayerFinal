var mongoose = require("mongoose");
var Schema = mongoose.Schema;

var PlayerSchema = new Schema({
    id:{
        type:String
    },
    score:{
        type:Number
    },
    name:{
        type:String
    }
});

mongoose.model("players", PlayerSchema);