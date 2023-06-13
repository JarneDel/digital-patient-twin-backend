# Meldingenservice
This service is used to get notifications from the database.

## Get notifications by patient
`/meldingen/:patientId`

## Get notifications by doctor
`/meldingen/dokter/:dokterid`

## filters
### Notification Level
`?level=number`
```md
Info: 0
Warning: 1
Ernstig: 2
All: 3
```

### Notification Type
`?type=number`
```md
Bloeddruk: 0
Temperatuur: 1
Hartslag: 2
Ademhaling: 3
Bloedzuurstof: 4
Alle: 5
```

### offset
`?offset=number`
```md
By default the offset is 0, you get 25 values back.
If you want to get the next 25 values you need to add 25 to the offset.
```