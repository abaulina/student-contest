// import { useNavigate } from 'react-router-dom';

// const navigate = useNavigate();

function handleError(response) {
	switch (response.status) {
		case 404: {
			//get user info - user not found - ???
			break;
		}
		case 401: {
			// invalid token, etc - ???
			throw Error(response.message);
		}
		default: {
			//navigate('/error');
			throw Error(response.message);
		}
	}
}

export default handleError;
