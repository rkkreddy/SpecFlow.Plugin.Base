Feature: SpecFlowFeature
	I want to load some excel stuffs

Scenario Outline: Core Load From Excel
	Given I have followed the instructions
	Then I should've loaded <One> <Two> <Three>

	@excel:Book1.xlsx:Sheet1
	Examples: 
	| One | Two | Three |