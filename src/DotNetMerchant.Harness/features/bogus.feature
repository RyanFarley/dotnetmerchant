Feature: Bogus Resource
	In order test the base controller functionality
	As a REST Client
	I want to be able to initiate credit card transactions and receive status messages

Scenario: Authorize endpoint of the Bogus resource should validate a valid transaction and return xml
	Given the following parameters
		| name         | value            |
		| number       | 1                |
        | amount       | 1000             |
        | test         | true             |
		When I post to the Authorize endpoint of the Bogus resource as xml
		Then the response code should indicate success 
		And the response should contain "<authorization>"
        And the response should contain "<approved>true</approved>"

		