import src.templates.template_reader as tr



def email_verification_mail(url :str) -> str:
    """
    HTML email verify message.
    Args:
        url (str): email verification url
    """
    return tr.read_template('email_verification_message.html', {'((url))': url})


def account_del_mail(url :str) -> str:
    """
    HTML account delete confirmation message.
    Args:
        url (str): account delete url
    """
    return tr.read_template('email_delete_account_confirmation.html', {'((url))': url})


def url_success_page(message :str) -> str:
    """
    HTML success page
    Args:
        message (str): success message
    """
    return tr.read_template('url_result_success.html', {'((message))': message})


def url_error_page(message :str) -> str:
    """
    HTML error page
    Args:
        message (str): error message
    """
    return tr.read_template('url_result_error.html', {'((message))': message})


def user_block_message() -> str:
    """
    HTML user block message
    """
    return tr.read_template('user_block_message.html', {})


def incorrect_login_message(ip :str) -> str:
    """
    HTML incorrect login message
    Args:
        ip (str): incorrect login ip address
    """
    return tr.read_template('email_incorrect_login.html', {'((ip))': ip})
