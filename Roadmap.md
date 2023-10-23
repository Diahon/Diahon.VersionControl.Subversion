# Roadmap

❌ = Nothing    
🟡 = Working under specific conditions (e.g. missing error handling)    
🟢 = Stable    

## Implemented Mechanics
| Mechanic | Implementation Status |
| --- | --- |
| Basic auth | 🟡 |
| Server-Capabilities | ❌ |
| Client-Capabilities | ❌ |
| Repo-Capabilities | ❌ |
| ... | ❌ |

## Implemented Commands

### Connection Establishment
| Command | Implementation Status |
| --- | --- |
| greeting | 🟡 |
| greeting-response | 🟡 |
| auth-request | 🟡 |
| auth-response | 🟡 |
| challenge | 🟡 |
| repos-info | 🟡 |

### Main Command Set
| Command | Implementation Status |
| --- | --- |
| reparent | ❌ |
| get-latest-rev | ❌ |
| get-dated-rev | ❌ |
| change-rev-prop | ❌ |
| change-rev-prop2 | ❌ |
| rev-proplist | ❌ |
| rev-prop | ❌ |
| commit | ❌ |
| get-file (cat) | 🟡 |
| get-dir | ❌ |
| check-path | ❌ |
| stat | ❌ |
| get-mergeinfo | ❌ |
| update | ❌ |
| switch | ❌ |
| status | ❌ |
| diff | ❌ |
| log | ❌ |
| get-locations | ❌ |
| get-location-segments | ❌ |
| get-file-revs | ❌ |
| lock | ❌ |
| lock-many | ❌ |
| unlock | ❌ |
| unlock-many | ❌ |
| get-lock | ❌ |
| get-locks | ❌ |
| replay | ❌ |
| replay-range | ❌ |
| get-deleted-rev | ❌ |
| get-iprops | ❌ |
| list | 🟡 |

### Editor Command Set
| Command | Implementation Status |
| --- | --- |
| target-rev | ❌ |
| open-root | ❌ |
| delete-entry | ❌ |
| add-dir | ❌ |
| open-dir | ❌ |
| change-dir-prop | ❌ |
| close-dir | ❌ |
| absent-dir | ❌ |
| add-file | ❌ |
| open-file | ❌ |
| apply-textdelta | ❌ |
| textdelta-chunk | ❌ |
| textdelta-end | ❌ |
| change-file-prop | ❌ |
| close-file | ❌ |
| absent-file | ❌ |
| close-edit | ❌ |
| abort-edit | ❌ |
| finish-replay | ❌ |

### Report Command Set
| Command | Implementation Status |
| --- | --- |
| set-path | ❌ |
| delete-path | ❌ |
| link-path | ❌ |
| finish-report | ❌ |
| abort-report | ❌ |
