require_relative '../../init.rb'

module LarsOfTheStars
	class Service < Sinatra::Application
        get '/register' do
            data = connect.from(:Sessions).where{ self.User == params['discID'] }
            user = data.first
            hash = { "Success" => false }
            if user.nil?
                hash = { "Success" => true }
                hash["Session"] = connect.from(:sessions).insert({
                    :User => params['discID'],
                    :Name => params['discName'],
                    :Disco => params['discNum'],
                    :Score => 0
                })
                data = connect.from(:Invasions).where{ self.Finish > Time.now.to_i and self.Start < Time.now.to_i }.all
                if data.empty?
                    hash["Defend"] = false
                else
                    hash["Defend"] = true
                end
            end
            json hash
        end
        get '/kill' do
            enemy = connect.from(:Enemies).where{ self.Spawn <= Time.now.to_i }.first
            hash = { "Success" => false }
            if not enemy.nil?
                connect.from(:Enemies).where{ self.Enemy == enemy[:Enemy] }.update({
                    :Status => "Won"
                })
                connect.from(:Sessions).where{ self.Session == params['session'] }.delete
                hash = { "Success" => true }
            end
            json hash
        end
        get '/setScore' do
            enemy = connect.from(:Enemies).where{ self.Enemy == params['cause'] }.first
            hash = { "Success" => true }
            if not enemy.nil?
                if enemy[:Status].nil?
                    amount = params['amount'].to_i
                    connect.from(:Enemies).where{ self.Enemy == enemy[:Enemy] }.update({
                        :Status => (amount < 0 ? "Won" : "Lost")
                    })
                end
            end
            user = connect.from(:Session).where{ self.Session == params['session'] }.first
            if not user.nil?
                amount = params['amount'].to_i
                connect.from(:Sessions).where{ self.Session == params['session'] }.update({
                    :Score => user[:Score] + amount
                })
            end
            json hash
        end
        get '/getScore' do
            data = connect.from(:Session).where{ self.Session == params['session'] }
            data = data.all.sort_by do |value|
                value[:Score]
            end
            json data
        end
        get '/getRanks' do
            data = connect.from(:Session)
            data = data.all.sort_by do |value|
                value[:Score]
            end
            json data
        end
    end
end