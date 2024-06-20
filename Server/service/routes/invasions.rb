require_relative '../../init.rb'

module LarsOfTheStars
	class Service < Sinatra::Application
        get '/getInvasions' do
            data = connect.from(:Invasions).where{ self.Finish > Time.now.to_i and self.Start < Time.now.to_i }
            data = data.all.sort_by do |value|
                value[:Finish]
            end
            json data
        end
        get '/getFutureInvasions' do
            data = connect.from(:Invasions).where{ self.Start > Time.now.to_i }
            data = data.all.sort_by do |value|
                value[:Finish]
            end
            json data
        end
        get '/getPastInvasions' do
            data = connect.from(:Invasions).where{ self.Start < Time.now.to_i }
            data = data.all.sort_by do |value|
                value[:Finish]
            end
            json data
        end
        get '/getAllInvasions' do
            data = connect.from(:Invasions)
            data = data.all.sort_by do |value|
                value[:Finish]
            end
            json data
        end
    end
end