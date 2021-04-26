const sendMessage = (type, body) => {
    window.external.sendMessage(JSON.stringify({Type:type, Body: body}))
}

const dispatcher = new Map()

const onMsg = (type, callback) => {
    dispatcher[type] = callback
}

window.external.receiveMessage(message => {
    const msg = JSON.parse(message)
    dispatcher[msg.Type](msg.Body)
})


const onFilterClick = (filter, e) => {
    if (e.target.tagName == 'A')
    {
        for(const atag of filter.getElementsByTagName('A'))
        {
            atag.className = atag.className.replace(" is-active", "")
        }
        e.target.className += " is-active"
    }
}

const filterTarget = {
    title: "Src file",
    args: [
        "2021-04-20-april3_dailylife.md",
        "2021-04-25-fsharp_de_photino.md",
        "2021-04-24-GUITool_like_sh.md"
    ]
}

const buildListHtml = (files) => {
    const htmls = []

    for( const [idx, file] of files.entries() ) {
        htmls.push(`<a class="panel-block filter-item" findex="${idx}">
    <span class="panel-icon">
    </span>
    ${file}
</a>`)    
    }
    return htmls.join("\n")
}

const getFilterRoot = (column) => column.getElementsByClassName("filter-root")[0]
const getTextRoot = (column) => column.getElementsByClassName("text-root")[0]

const showFilter = (column) => {
    getFilterRoot(column).style.display = "block"
    getTextRoot(column).style.display = "none"
}
const hideBoth = (column) => {
    getFilterRoot(column).style.display = "none"
    getTextRoot(column).style.display = "none"
}

const bindFilterData = (panel, title, texts) => {
    showFilter(panel)
    panel.getElementsByClassName("panel-heading")[0].innerText = title
    panel.getElementsByClassName("list-root")[0].innerHTML = buildListHtml(texts)
}

const showText = (panel, title) => {
    panel.getElementsByClassName("panel-heading")[0].innerText = title
    getFilterRoot(panel).style.display = "none"
    getTextRoot(panel).style.display = "block"
}



const COLUMN_TYPE_FILTER=1
const COLUMN_TYPE_TEXT=2
const COLUMN_TYPE_NONE=3

window.addEventListener('load', (e)=> {
    const leftColumn = document.getElementById('left-column')
    const rightColumn = document.getElementById('right-column')

    const getTextInput = (column) => getTextRoot(column).getElementsByTagName("input")[0]
    const getSearchInput = (column) => getFilterRoot(column).getElementsByTagName("input")[0]

    const setupColumn = (column) => {
        column.addEventListener('click',
                (e)=>onFilterClick(column, e))

        getSearchInput(column).addEventListener('input', (e)=> {
            filterColumn(column, e.target.value)
        })

        getTextInput(column).addEventListener('keydown', (e)=> {
            if(e.keyCode === 13) { // enter, without IME.
                gotoNext(column)
                return
            }
        })
    }


    setupColumn(leftColumn)
    setupColumn(rightColumn)

    const columnDataMap = new Map()

    const bindData = (column, datamsg) => {
        if(datamsg.Type == COLUMN_TYPE_FILTER) {
            bindFilterData(column, datamsg.Title, datamsg.Args)
        } else { // TEXT
            showText(column, datamsg.Title)
        } 
        columnDataMap[column.id] = datamsg
    }    

    const filterColumn = (column, pat) => {
        const arrays = columnDataMap[column.id].Args
        const matches = arrays.map(elem => elem.includes(pat))
        const root = column.getElementsByClassName("list-root")[0]
        const items = root.getElementsByClassName("filter-item")
        for(const [idx, flag] of matches.entries())
        {
            items[idx].className = items[idx].className.replace(" filtered", "")
            if(!flag) {
                items[idx].className += " filtered"
            }
        }
    }


    let leftType = COLUMN_TYPE_NONE
    let rightType = COLUMN_TYPE_NONE

    const gotoNext = (column) => {
        if(column == leftColumn && rightType != COLUMN_TYPE_NONE) {
            focusColumn(rightColumn, rightType)
            return
        }
        onSubmit()
    }


    const focusColumn = (column, columnType)=> {
        if (columnType == COLUMN_TYPE_FILTER) {
            getSearchInput(column).focus()
        } else { // TEXT
            getTextInput(column).focus()
        }     
    }


    let leftData = null
    let rightData = null

    onMsg("bindData", (body) => {
        const datas = JSON.parse(body)
        leftType = datas[0].Type
        leftData = datas[0]
        bindData(leftColumn, leftData)

        if (datas.length == 1) {
            rightType = COLUMN_TYPE_NONE
            rightColumn.style.display = "none"
        } else {
            rightType = datas[1].Type
            rightData = datas[1]
            bindData(rightColumn, datas[1])
        }
        focusColumn(leftColumn, leftType)
    })

    const getResult = (column, data) => {
        if (data.Type == COLUMN_TYPE_FILTER) {
            const actives = column.getElementsByClassName("is-active")
            if (actives.length == 0) {
                throw 'No selection!'
            }
            const files = data.Args
            return files[parseInt(actives[0].getAttribute("findex"))]
        } else {// TEXT
            const res = column.getElementsByClassName("text-root")[0].getElementsByClassName("input")[0].value
            if (res == "")
                throw 'No input!'
            return res
        }
    }

    const onSubmit = ()=> {
        try {
            let leftRes = getResult(leftColumn, leftData)
            if (rightType == COLUMN_TYPE_NONE) {
                sendMessage("notifySubmit", JSON.stringify([leftRes]))
            } else {
                let rightRes = getResult(rightColumn, rightData)
                sendMessage("notifySubmit", JSON.stringify([leftRes, rightRes]))
            }
        }catch(e) {
            alert(e)
        }
    }

    document.getElementById('submit-button').addEventListener('click', onSubmit)
    document.getElementById('cancel-button').addEventListener('click', ()=>sendMessage('notifyCancel', ""))


    sendMessage("notifyLoaded", "")
})

