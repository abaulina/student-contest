import history from '../utilities/history';

function handleError(responseStatus) {
	switch (responseStatus) {
		case '404': {
			history.push('/notfound');
			break;
		}
		case '401':
			return false;
		default: {
			history.push('/error');
		}
	}
}

export default handleError;
