module NavigationHelpers
  # Maps a name to a path. Used by the
  #
  #   When /^I go to (.+)$/ do |page_name|
  #
  # step definition in web_steps.rb
  #
  def path_to(page_name)
    case page_name
    
	when /the version endpoint as xml/
	  '/version.xml'
	when /the version endpoint as json/
	  '/version.json'
<<<<<<< HEAD:src/DotNetMerchant.Harness/features/support/paths.rb
	when /the VerifyCreditCard endpoint as xml/
	  '/verify_credit_card.xml'
	 when /the VerifyCreditCard endpoint as json/
	  '/verify_credit_card.json'
=======
>>>>>>> ea800715a1906ab57c328dc1f4a15f7ad0c42273:src/DotNetMerchant.Harness/features/support/paths.rb
    # Add more mappings here.
    # Here is an example that pulls values out of the Regexp:
    #
    #   when /^(.*)'s profile page$/i
    #     user_profile_path(User.find_by_login($1))

    else
      raise "Can't find mapping from \"#{page_name}\" to a path.\n" +
        "Now, go and add a mapping in #{__FILE__}"
    end
  end
end

World(NavigationHelpers)
