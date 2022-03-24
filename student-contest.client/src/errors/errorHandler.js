import toast from 'react-hot-toast';
import { useNavigate } from 'react-router-dom';

const navigate = useNavigate();

function handleError(response) {
	switch (response.status) {
		case 409: {
			toast.error('Email is invalid');
			break;
		}
		case 404: {
			//get user info - user not found - ???
			break;
		}
		case 401: {
			// invalid token, etc - ???
			break;
		}
		default: {
			navigate('/error');
			throw Error(response.message);
		}
	}
}

export default handleError;
