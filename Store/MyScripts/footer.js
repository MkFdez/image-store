function adjustFooterPosition() {
    const bodyHeight = document.body.clientHeight;
    const windowHeight = window.innerHeight;
    const footerHeight = document.querySelector('.footer').clientHeight;
    console.log('here' + bodyHeight + " " + footerHeight)
    if (bodyHeight + footerHeight < windowHeight) {
        console.log('here'+ bodyHeight +" "+ footerHeight)
        document.querySelector('.footer').style.position = 'absolute';
        document.querySelector('.footer').style.bottom = '0';
    } else {
        console.log('here2')
        document.querySelector('.footer').style.position = 'static';
    }
}

// Adjust footer position initially and on window resize
window.addEventListener('resize', adjustFooterPosition);
window.addEventListener('DOMContentLoaded', adjustFooterPosition);