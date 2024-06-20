# Collect all references here.
require 'discordrb'
require 'securerandom'
require 'digest'
require 'sqlite3'
require 'sequel'
require 'oj'
require 'date'
require 'thread'

# Start service.
if __FILE__ == $0
    require_relative './service/main.rb'
    require_relative './service/game.rb'
end