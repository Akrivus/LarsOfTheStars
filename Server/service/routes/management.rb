require_relative '../../init.rb'

module LarsOfTheStars
	class Service < Sinatra::Application
        get '/getEnemies' do
            data = connect.from(:Enemies).where{ self.Spawn == Time.now.to_i }
            data = data.all.sort_by do |value|
                value[:Spawn]
            end
            json data
        end
        get '/getPastEnemies' do
            data = connect.from(:Enemies).where{ self.Spawn <= Time.now.to_i }
            data = data.all.sort_by do |value|
                value[:Spawn]
            end
            json data
        end
    end
end