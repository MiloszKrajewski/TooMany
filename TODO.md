# Proto

* Remoting serialization
* Persistence
* SQL Lite ?

# Actors

* Task
    * identified by guid
    * name, state, expected state, definition
    
* Registry
    * list of tasks name -> guid
    
# Commands
start <name>
stop <name>
restart <name>
define <name> <def>
