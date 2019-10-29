#!/usr/bin/env ruby
require 'sii_chile'
print SIIChile::Consulta.new(ARGV[0]).resultado