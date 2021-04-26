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
        htmls.push(`<a class="panel-block" findex="${idx}">
    <span class="panel-icon">
    </span>
    ${file}
</a>`)    
    }
    return htmls.join("\n")
}

const getFilterRoot = (panel) => panel.getElementsByClassName("filter-root")[0]
const getTextRoot = (panel) => panel.getElementsByClassName("text-root")[0]

const showFilter = (panel) => {
    getFilterRoot(panel).style.display = "block"
    getTextRoot(panel).style.display = "none"
}
const hideBoth = (panel) => {
    getFilterRoot(panel).style.display = "none"
    getTextRoot(panel).style.display = "none"
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

const setupColumn = (column) => {
    column.addEventListener('click',
            (e)=>onFilterClick(column, e))

}


const COLUMN_TYPE_FILTER=1
const COLUMN_TYPE_TEXT=2
const COLUMN_TYPE_NONE=3

window.addEventListener('load', (e)=> {
    const leftColumn = document.getElementById('left-column')
    const rightColumn = document.getElementById('right-column')

    setupColumn(leftColumn)
    setupColumn(rightColumn)

    const bindData = (column, datamsg) => {
        if(datamsg.Type == COLUMN_TYPE_FILTER) {
            bindFilterData(column, datamsg.Title, datamsg.Args)
        } else { // TEXT
            showText(column, datamsg.Title)
        } 
    }
    

    let leftType = COLUMN_TYPE_NONE
    let rightType = COLUMN_TYPE_NONE

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

