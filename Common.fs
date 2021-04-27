module Common

// DataMessage
// Always send as DataMesssage array.
// first element is data for left column, second element is for right.

let COLUMN_TYPE_FILTER = 1
let COLUMN_TYPE_TEXT = 2
let COLUMN_TYPE_NONE = 3

type DataMessage = {Type: int; Title:string; Args: string array}
