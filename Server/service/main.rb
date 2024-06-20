require_relative '../init.rb'
require 'sinatra'

module LarsOfTheStars
	class Service < Sinatra::Application
        set :server, "webrick"
        set :bind, "0.0.0.0"
        set :port, 2633
        
        require_relative './routes/invasions.rb'
        require_relative './routes/management.rb'
        require_relative './routes/sessions.rb'
        
        before do
            request.body.rewind
            build_db!
        end
        
        def json(hash)
            Oj.dump hash
        end
        def connect
            Sequel.connect('sqlite://service/private/records.db')
        end
        def build_db!
            db = regen_db!
            db.create_table? :Invasions do
                Integer :ID,    :primary_key => true
                Integer :Start
                Integer :Finish
                Integer :Count
                String  :Color
                String  :Variety
            end
            db
        end
        def clean_db!
            db = connect
            db.drop_table? :Sessions
            db.drop_table? :Enemies
            db
        end
        def regen_db!
            db = connect
            db.create_table? :Sessions do
                Integer :ID,    :primary_key => true
                Integer :User
                String  :Name
                String  :Disco
                Integer :Score
            end
            db.create_table? :Enemies do
                Integer :ID,    :primary_key => true
                Integer :Spawn
                Float   :StartX
                Float   :StartY
                Float   :Angle
                String  :Color
                String  :Status
            end
            db
        end
        run!
    end
end