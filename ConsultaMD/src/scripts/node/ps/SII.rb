#!/usr/bin/env ruby
require 'sii_chile'
require 'json'
sii = SIIChile::Consulta.new(ARGV[0]).resultado
print sii.to_json