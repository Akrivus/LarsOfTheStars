require_relative '../init.rb'

module LarsOfTheStars
    class Discord
        $bot = Discordrb::Bot.new({
            :token => ENV["DISCORD_API_TOKEN"],
            :client_id => 168123456789123456
        })
        $bot.mention do |event|
            args = event.text.split(" ")
            if args[1] == "invasion"
                id = $DB.from(:Invasions).insert({
                    :Start => args[2].to_i,
                    :Finish => args[3].to_i,
                    :Count => args[4].to_i,
                    :Color => args[5],
                    :Variety => "Random"
                })
                event.respond "Created invasion ##{id} " +
                    "starting at #{DateTime.strptime(args[2], '%s')} " +
                    "until #{DateTime.strptime(args[3], '%s')} " +
                    "with #{args[4]} #{args[5]} ships."
            end
        end
        $bot.message do |event|
            if event.text == "ping"
                event.respond "pong"
            end
        end
        $bot.run :async
        $invasion_message = $bot.send_message(492029496959041537, "Loading...")
        $invasion_embed_hash = {
            "title" => "**__Current Invasions__**",
            "description" => "There are __0__ ongoing invasions.",
            "fields" => [],
            "footer": {
              "text": "Updated #{Time.now.strftime("%l:%M:%S %p, %Z")}."
            }
        }
        $running = Thread.new do 
            while not $running.nil?
                $DB = Sequel.connect('sqlite://service/private/records.db')
                
                # Update invasion embed.
                data = $DB.from(:Invasions).where{ self.Finish > Time.now.to_i and self.Start < Time.now.to_i }
                data = data.all.sort_by do |value|
                    value[:Finish]
                end
                
                # Update the hash.
                $invasion_embed_hash["description"] = "There are __#{data.length.to_s}__ ongoing invasions."
                data.each do |piece|
                    $invasion_embed_hash["fields"].push({
                        "name" => "#{data[:Variety]} #{data[:Color].capitalize} Invasion",
                        "value" => "#{data[:Count]} total, <%=math shit i need to put here%>"
                    })
                end
                
                # Push updates.
                $invasion_embed_hash["footer"] = "Updated #{Time.now.strftime("%l:%M:%S %p, %Z")}."
                $invasion_message.edit("", $invasion_embed_hash)
                
                # Finalizer stuff.
                $bot.game = "God"
                sleep 1
            end
        end
    end
end