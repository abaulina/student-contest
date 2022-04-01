// import { useNavigate } from 'react-router-dom';

function handleError(responseStatus) {
	// const navigate = useNavigate();
	switch (responseStatus) {
		case 404: {
			// navigate('/notfound');
			break;
		}
		case 401:
			return false;
		default: {
			// navigate('/error');
		}
	}
}

export default handleError;
